using System;
using System.Linq;
using System.Threading.Tasks;
using GitNStats.Core;
using LibGit2Sharp;

namespace GitNStats
{
    public enum Result
    {
        Success = 0,
        Failure = 1
    }
    
    public class Controller
    {
        private readonly View _view;
        private readonly FileSystem _fileSystem;
        
        private readonly Func<string, IRepository> RepositoryFactory;
        private readonly Func<IRepository, AsyncVisitor> AsyncVisitorFactory;

        public Controller(View view, FileSystem fileSystem, Func<string, IRepository> repositoryFactory, Func<IRepository, AsyncVisitor> asyncVisitorFactory)
        {
            _view = view;
            _fileSystem = fileSystem;
            RepositoryFactory = repositoryFactory;
            AsyncVisitorFactory = asyncVisitorFactory;
        }

        public Task<Result> RunAnalysis(Options options)
        {
            var repoPath = RepositoryPath(options);
            var filter = Filter(options.DateFilter);
            
            return RunAnalysis(repoPath, options.BranchName, filter);
        }

        private async Task<Result> RunAnalysis(string repositoryPath, string branchName, Func<(Commit, TreeEntryChanges), bool> filter)
        {
            try
            {
                using (var repo = RepositoryFactory(repositoryPath))
                {
                    var branch = Branch(branchName, repo);
                    if (branch == null)
                    {
                        _view.DisplayError($"Invalid branch: {branchName}");
                        return Result.Failure;
                    }

                    _view.DisplayRepositoryInfo(repositoryPath, branch);

                    var diffs = await AsyncVisitorFactory(repo).Walk(branch.Tip);
                    var filteredDiffs = diffs.Where(filter);
                    
                    _view.DisplayPathCounts(Analysis.CountFileChanges(filteredDiffs));
                    return Result.Success;
                }
            }
            catch (RepositoryNotFoundException)
            {
                _view.DisplayError($"{repositoryPath} is not a git repository.");
                return Result.Failure;
            }
        }

        private string RepositoryPath(Options options)
        {
            return String.IsNullOrWhiteSpace(options.RepositoryPath)
                ? _fileSystem.CurrentDirectory()
                : options.RepositoryPath;
        }
        
        private static Func<(Commit, TreeEntryChanges), bool> Filter(DateTime? dateFilter)
        {
            bool NoFilter((Commit, TreeEntryChanges) diffs) => true;

            return (dateFilter == null)
                ? NoFilter
                : Analysis.OnOrAfter(DateTime.SpecifyKind(dateFilter.Value, DateTimeKind.Local));
            
            // Datetime may come in in as "unspecified", we need to be sure it's specified 
            // to get accurate comparisons to a commit's DateTimeOffset
        }
        
        private static Branch Branch(string branchName, IRepository repo)
        {
            // returns null if branch name is specified but doesn't exist
            return (branchName == null) ? repo.Head : repo.Branches[branchName];
        }
    }
}