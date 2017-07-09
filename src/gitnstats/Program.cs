using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using CommandLine;

namespace GitNStats
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunAnalysis(options.RepositoryPath));
        }

        private static void RunAnalysis(string repositoryPath)
        {
            //TODO: handle non-existant repo exception
            using (var repo = new Repository(repositoryPath))
            {
                var changeCounts = new ConcurrentDictionary<String, int>();

                var visitor = new CommitVisitor();
                visitor.Visited += (sender, visited) =>
                {
                    foreach (var parent in visited.Parents)
                    {
                        var diff = repo.Diff.Compare<TreeChanges>(parent.Tree, visited.Tree);

                        foreach (var changed in diff)
                        {
                            var path = changed.Path;
                            changeCounts.AddOrUpdate(path, 1, (id, count) => count + 1);
                        }
                    }
                };

                visitor.Walk(repo.Head.Tip);

                Console.WriteLine("Change Count\tPath");
                var sortedCounts = changeCounts.OrderByDescending(rec => rec.Value);
                foreach (var count in sortedCounts)
                {
                    Console.WriteLine($"{count.Value}\t{count.Key}");
                }
            }
        }
    }
}
