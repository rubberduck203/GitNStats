using System;
using System.Collections.Generic;
using LibGit2Sharp;
using System.IO;
using System.Threading.Tasks;
using CommandLine;

namespace GitNStats
{
    static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(options =>
                {
                    var repoPath = String.IsNullOrWhiteSpace(options.RepositoryPath)
                        ? Directory.GetCurrentDirectory()
                        : options.RepositoryPath;

                    if (options.DateFilter.HasValue)
                    {
                        // Comes in as "unspecified", we need to be sure it's specified 
                        // to get accurate comparisons to a commit's DateTimeOffset
                        options.DateFilter = DateTime.SpecifyKind(options.DateFilter.Value, DateTimeKind.Local);
                    }
                    
                    return RunAnalysis(repoPath, options.BranchName, options.DateFilter).Result;
                }, _ => 1);
        }

        private static async Task<int> RunAnalysis(string repositoryPath, string branchName, DateTime? dateFilter)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var branch = (branchName == null) ? repo.Head : repo.Branches[branchName];
                    if (branch == null)
                    {
                        WriteError($"Invalid branch: {branchName}");
                        return 1;
                    }

                    PrintRepositoryInfo(repositoryPath, branch);

                    var walker = new DiffCollector(new CommitVisitor(), new DiffListener(repo));
                    var diffs = await walker.Walk(branch.Tip);

                    if (dateFilter.HasValue)
                    {
                        diffs = diffs.OnOrAfter(dateFilter.Value);
                    }
                    
                    PrintPathCounts(Analysis.CountFileChanges(diffs));
                    return 0;
                }
            }
            catch (RepositoryNotFoundException)
            {
                WriteError($"{repositoryPath} is not a git repository.");
                return 1;
            }
        }

        private static void PrintPathCounts(IEnumerable<PathCount> pathCounts)
        {
            Console.WriteLine("Commits\tPath");
            foreach (var summary in pathCounts)
            {
                Console.WriteLine($"{summary.Count}\t{summary.Path}");
            }
        }

        private static void PrintRepositoryInfo(string repositoryPath, Branch branch)
        {
            Console.WriteLine($"Repository: {repositoryPath}");
            Console.WriteLine($"Branch: {branch.FriendlyName}");
            Console.WriteLine();
        }
        
        private static void WriteError(string message)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                Console.Error.WriteLine(message);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }
    }
}