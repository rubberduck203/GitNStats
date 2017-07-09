using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
                    
                    var changeCounts = new ConcurrentDictionary<String, int>();
                    void OnVisited(object sender, Commit visited)
                    {
                        foreach (var parent in visited.Parents)
                        {
                            var diff = repo.Diff.Compare<TreeChanges>(parent.Tree, visited.Tree);

                            foreach (var changed in diff)
                            {
                                changeCounts.AddOrUpdate(changed.Path, 1, (id, count) => count + 1);
                            }
                        }
                    }

                    var visitor = new CommitVisitor();
                    visitor.Visited += OnVisited;

                    Console.WriteLine($"Repository: {repositoryPath}");
                    Console.WriteLine($"Branch: {branch.FriendlyName}");
                    Console.WriteLine();

                    visitor.Walk(branch.Tip);

                    var sortedCounts = changeCounts.OrderByDescending(rec => rec.Value);
                    Console.WriteLine("Commits\tPath");
                    foreach (var count in sortedCounts)
                    {
                        Console.WriteLine($"{count.Value}\t{count.Key}");
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
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                Console.Error.WriteLine(message);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Black;
            }
        }
    }
}