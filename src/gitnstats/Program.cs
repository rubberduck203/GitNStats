using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace gitnstats
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Git'n'Stats: Git History Analyzer");

            //TODO: handle non-existant repo exception
            using (var repo = new Repository(@"/Users/rubberduck/Documents/Source/theupsyde/"))
            {
                foreach (var commit in repo.Commits.Take(1))
                {
                    Console.Write(commit.Sha + "\t");
                    Console.WriteLine(commit.MessageShort);

                    PrintFilePathsAt(commit);
                }
            }
        }

        private static void PrintFilePathsAt(Commit commit)
        {
            foreach (var treeEntry in Flatten(commit.Tree))
            {
                Console.Write("\t");
                Console.Write(treeEntry.Path + "\t");
                Console.WriteLine(treeEntry.TargetType.ToString() + "\t");
            }
        }

        private static IEnumerable<TreeEntry> Flatten(Tree tree)
        {
            foreach (var treeEntry in tree)
            {
                switch (treeEntry.TargetType)
                {
                    case TreeEntryTargetType.Blob:
                    case TreeEntryTargetType.GitLink:
                        yield return treeEntry;
                        break;
                    case TreeEntryTargetType.Tree:
                        foreach (var innerEntry in Flatten((Tree)treeEntry.Target))
                        {
                            yield return innerEntry;
                        }
                        break;
                }
            }
        }
    }
}
