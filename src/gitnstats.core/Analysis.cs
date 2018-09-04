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
            return diffs.Aggregate<(Commit Commit, TreeEntryChanges Diff), Dictionary<string, int>>(
                new Dictionary<string, int>(), //filename, count
                (acc, x) =>
                    acc.Where(kv => kv.Key != x.Diff.Path)
                        .Union(Enumerable.Repeat(new KeyValuePair<string, int>(x.Diff.Path, acc.GetOrDefault(x.Diff.OldPath, 0) + 1), 1))
                        .Where(kv => x.Diff.Status != ChangeKind.Renamed || (x.Diff.Status == ChangeKind.Renamed && kv.Key != x.Diff.OldPath))
                        .ToDictionary(kv => kv.Key, kv => kv.Value)

            )
            .Select(x => new PathCount(x.Key, x.Value))
            .OrderByDescending(s => s.Count);
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

    static class DictionaryExtensions
    {
        public static V GetOrDefault<K,V>(this Dictionary<K,V> dictionary, K key, V defaultValue)
        {
            return dictionary.TryGetValue(key, out V value) ? value : defaultValue;
        }
    }
}