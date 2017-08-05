using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitNStats.Core
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

        /// <summary>
        /// Predicate for filtering by date
        /// </summary>
        /// <param name="onOrAfter">Local DateTime</param>
        /// <returns>True if Commit was on or after <paramref name="onOrAfter"/>, otherwise false.</returns>
        public static Func<(Commit Commit, TreeEntryChanges Diff), bool> OnOrAfter(DateTime onOrAfter)
        {
            /* 
             * A Commit could have been recorded in any time zone.
             * We need to convert all DateTimes to UTC to get an accurate comparison.
             */
            return change => change.Commit.Author.When.ToUniversalTime() >= onOrAfter.ToUniversalTime();
        }
    }
}