using System.Collections.Generic;
using GitNStats.Core;
using LibGit2Sharp;

using PathCount = GitNStats.CommitAnalysis.FileChanges.PathCount;

namespace GitNStats
{
    public interface View
    {
        bool QuietMode { get; set; }
        void DisplayRepositoryInfo(string repositoryPath, Branch branch);
        void DisplayPathCounts(IEnumerable<PathCount> pathCounts);
        void DisplayError(string message);
    }
}