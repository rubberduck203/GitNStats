using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using CommandLine;
using LibGit2Sharp;

namespace GitNStats
{
    class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(options =>
                {
                    var repoPath = String.IsNullOrWhiteSpace(options.RepositoryPath)
                        ? Directory.GetCurrentDirectory()
                        : options.RepositoryPath;
                    
                    return RunAnalysis(repoPath, options.BranchName);
                }, _ => 1);
        }

        private static int RunAnalysis(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new LibGit2Sharp.Repository(repositoryPath))
                {
                    var branch = (branchName == null) ? repo.Head : repo.Branches[branchName];
                    if (branch == null)
                    {
                        WriteError($"Invalid branch: {branchName}");
                        return 1;
                    }

                    Console.WriteLine($"Repository: {repositoryPath}");
                    Console.WriteLine($"Branch: {branch.FriendlyName}");
                    Console.WriteLine();
                    
                    var listener = new DiffListener(repo);
                    var visitor = new CommitVisitor();
                    visitor.Visited += listener.OnCommitVisited;

                    visitor.Walk(branch.Tip);
                    
                    var changeCounts = listener.Diffs
                        .Select(d => d.Item2)
                        .GroupBy(c => c.Path)
                        .Select(x => new {Path = x.Key, Count = x.Count()})
                        .OrderByDescending(s => s.Count);

                    Console.WriteLine("Commits\tPath");
                    foreach (var summary in changeCounts)
                    {
                        Console.WriteLine($"{summary.Count}\t{summary.Path}");
                    }

//                    var data = listener.Diffs
//                        .Select(d =>
//                        {
//                            var (commit, diff) = d;
//                            return new {commit.Author, diff.Path};
//                        })
//                        .GroupBy(x => new {x.Author.When.Date, x.Path})
//                        .Select(x => new {x.Key.Path, x.Key.Date, Count = x.Count()});
//
//                    foreach (var change in data)
//                    {
//                        Console.WriteLine($"{change.Date}\t{change.Count}\t{change.Path}");
//                    }
                    return 0;
                }
            }
            catch (LibGit2Sharp.RepositoryNotFoundException)
            {
                WriteError($"{repositoryPath} is not a git repository.");
                return 1;
            }
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