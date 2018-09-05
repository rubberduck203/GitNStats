using System;
using System.Collections.Generic;
using GitNStats.Core;
using LibGit2Sharp;

using PathCount = GitNStats.CommitAnalysis.FileChanges.PathCount;

namespace GitNStats
{
    public class CliView : View
    {
        public bool QuietMode { get; set; }

        public void DisplayRepositoryInfo(string repositoryPath, Branch branch)
        {
            if (QuietMode) return;

            Console.WriteLine($"Repository: {repositoryPath}");
            Console.WriteLine($"Branch: {branch.FriendlyName}");
            Console.WriteLine();
        }

        public void DisplayPathCounts(IEnumerable<PathCount> pathCounts)
        {
            if (!QuietMode)
            {
                Console.WriteLine("Commits\tPath");
            }

            foreach (var summary in pathCounts)
            {
                Console.WriteLine($"{summary.Count}\t{summary.Path}");
            }
        }
        
        public void DisplayError(string message)
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