using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

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
                foreach (var commit in repo.Commits.Take(1))
                {
                    PrintCommitInfo(commit);
                    PrintFilePathsAt(commit);
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
