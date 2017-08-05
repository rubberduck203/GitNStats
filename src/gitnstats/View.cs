using System.Collections.Generic;
using GitNStats.Core;
using LibGit2Sharp;

namespace GitNStats
{
    public interface View
    {
        void DisplayRepositoryInfo(string repositoryPath, Branch branch);
        void DisplayPathCounts(IEnumerable<PathCount> pathCounts);
        void DisplayError(string message);
    }
}