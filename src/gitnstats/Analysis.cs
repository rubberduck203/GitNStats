using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitNStats
{
    public static class Analysis
    {
        public static IEnumerable<PathCount> CountFileChanges(IEnumerable<(Commit, TreeEntryChanges)> diffs)
        {
            return diffs
                .GroupBy<(Commit Commit, TreeEntryChanges Diff), string>(c => c.Diff.Path)
                .Select(x => new PathCount(x.Key, x.Count()))
                .OrderByDescending(s => s.Count);
        }
    }
}