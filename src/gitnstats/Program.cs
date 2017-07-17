using System;
using LibGit2Sharp;
using System.Linq;
using System.IO;
using CommandLine;

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
                using (var repo = new Repository(repositoryPath))
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
                        .GroupBy(c => c.Diff.Path)
                        .Select(x => new {Path = x.Key, Count = x.Count()})
                        .OrderByDescending(s => s.Count);

                    Console.WriteLine("Commits\tPath");
                    foreach (var summary in changeCounts)
                    {
                        Console.WriteLine($"{summary.Count}\t{summary.Path}");
                    }
                    return 0;
                }
            }
            catch (RepositoryNotFoundException)
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