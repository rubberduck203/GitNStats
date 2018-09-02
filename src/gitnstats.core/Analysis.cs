using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitNStats.Core
{
    public delegate bool DiffFilterPredicate((Commit Commit, TreeEntryChanges TreeEntryChanges) diff);
    
    public static class Analysis
    {
        public static IEnumerable<PathCount> CountFileChanges(IEnumerable<(Commit, TreeEntryChanges)> diffs)
        {
            // public static TResult Aggregate<TSource,TAccumulate,TResult> 
            // (this System.Collections.Generic.IEnumerable<TSource> source, 
            // TAccumulate seed, 
            // Func<TAccumulate,TSource,TAccumulate> func, 
            // Func<TAccumulate,TResult> resultSelector);

            return diffs.Aggregate<(Commit Commit, TreeEntryChanges Diff), Dictionary<string, int>>(
                new Dictionary<string, int>(),
                (acc, x) => {
                    if (x.Diff.Status == ChangeKind.Renamed) {
                        var oldCount = acc[x.Diff.OldPath];
                        acc.Remove(x.Diff.OldPath);
                        acc[x.Diff.Path] = oldCount + 1;
                    } else {
                        if (acc.TryGetValue(x.Diff.Path, out int count)) {
                            acc[x.Diff.Path] = count + 1;
                        } else {
                            acc[x.Diff.Path] = 1;
                        }
                    }
                    return acc;
                }
            ).Select(x => new PathCount(x.Key, x.Value))
            .OrderByDescending(s => s.Count);

            // return diffs
            //     .GroupBy<(Commit Commit, TreeEntryChanges Diff), string>(c => c.Diff.Path)
            //     .Select(x => new PathCount(x.Key, x.Count()))
            //     .OrderByDescending(s => s.Count);
        }

        /// <summary>
        /// Predicate for filtering by date
        /// </summary>
        /// <param name="onOrAfter">Local DateTime</param>
        /// <returns>True if Commit was on or after <paramref name="onOrAfter"/>, otherwise false.</returns>
        public static DiffFilterPredicate OnOrAfter(DateTime onOrAfter)
        {
            return change => change.Commit.Author.When.ToUniversalTime() >= onOrAfter.ToUniversalTime();
        }
    }
}