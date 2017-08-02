using System;
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

        public static IEnumerable<(Commit, TreeEntryChanges)> OnOrAfter(this IEnumerable<(Commit Commit, TreeEntryChanges Diff)> diffs, DateTime onOrAfter)
        {
            /* 
             * A Commit could have been recorded in any time zone.
             * We need to convert all DateTimes to UTC to get an accurate comparison.
             */
            return diffs.Where(d => d.Commit.Author.When.ToUniversalTime() >= onOrAfter.ToUniversalTime());
        }
    }
}