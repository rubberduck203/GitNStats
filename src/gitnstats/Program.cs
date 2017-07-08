using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GitNStats
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Git'n'Stats: Git History Analyzer");

            //TODO: handle non-existant repo exception
            using (var repo = new Repository(@"/Users/rubberduck/Documents/Source/theupsyde/"))
            {
                // foreach (var commit in repo.Commits.Take(10))
                // {
                //     PrintCommitTreeInfo(commit);
                // }

                // repo.Head.Tip.Walk(repo, PrintTreeDiffs);

                var changeCounts = new ConcurrentDictionary<String, int>();

                repo.Head.Tip.Walk((commit) =>
                {
                    foreach (var parent in commit.Parents)
                    {
                        var diff = repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree);

                        foreach (var changed in diff)
                        {
                            var path = changed.Path;
                            changeCounts.AddOrUpdate(path, 1, (id, count) => count + 1);
                        }
                    }
                });

                Console.WriteLine("Change Count\tPath");
                var sortedCounts = changeCounts.OrderByDescending(rec => rec.Value);
                foreach(var count in sortedCounts)
                {
                    Console.WriteLine($"{count.Value}\t{count.Key}");
                }
            }
        }

        private static void PrintTreeDiffs(Repository repo, Commit commit)
        {
            PrintCommitInfo(commit);
            foreach (var parent in commit.Parents)
            {
                var diff = repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree);

                foreach (var changed in diff)
                {
                    Console.Write(changed.Path + "\t");
                    Console.Write(changed.Mode + "\t");
                    Console.WriteLine(changed.Status);
                }
            }
        }

        private static void PrintCommitTreeInfo(Commit commit)
        {
            PrintCommitInfo(commit);
            PrintFilePathsAt(commit);
        }

        private static void PrintCommitInfo(Commit commit)
        {
            Console.Write(commit.Sha + "\t");
            Console.WriteLine(commit.MessageShort);
        }

        private static void PrintFilePathsAt(Commit commit)
        {
            foreach (var treeEntry in commit.Tree.Flatten())
            {
                Console.Write("\t");
                Console.Write(treeEntry.Path + "\t");
                Console.WriteLine(treeEntry.TargetType.ToString() + "\t");
            }
        }
    }
}
