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
            // Union must take an IEnumerable
            IEnumerable<KeyValuePair<K, V>> KeyValuePairEnumerable<K, V>(K key, V value) => 
                Enumerable.Repeat(new KeyValuePair<K, V>(key, value), 1);

            IEnumerable<KeyValuePair<string, int>> IncrementedPathCount(Dictionary<string, int> pathcounts, string currentPath, string lastPath) =>
                KeyValuePairEnumerable(currentPath, pathcounts.GetOrDefault(lastPath, 0) + 1);

            bool NotRenamed(KeyValuePair<string, int> kv, TreeEntryChanges diff) => 
                diff.Status != ChangeKind.Renamed || (diff.Status == ChangeKind.Renamed && kv.Key != diff.OldPath);

            return diffs.Aggregate<(Commit Commit, TreeEntryChanges Diff), Dictionary<string, int>>(
                new Dictionary<string, int>(), //filename, count
                (acc, x) =>
                    acc.Where(kv => kv.Key != x.Diff.Path)                              //All records except the current one
                        .Union(IncrementedPathCount(acc, x.Diff.Path, x.Diff.OldPath))  //Plus the current one, renamed if applicable
                        .Where(kv => NotRenamed(kv, x.Diff))                            //Strip away obsolete file names
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