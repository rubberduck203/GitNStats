using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

using GitNStats.CommitAnalysis;
using PathCount = GitNStats.CommitAnalysis.FileChanges.PathCount;

namespace GitNStats.Core
{
    public delegate bool DiffFilterPredicate((Commit Commit, TreeEntryChanges TreeEntryChanges) diff);
    
    public static class Analysis
    {
        public static IEnumerable<PathCount> CountFileChanges(IEnumerable<(Commit, TreeEntryChanges)> diffs)
        {
            var treeChanges = diffs.Select(x => x.Item2);
            return FileChanges.Count(treeChanges)
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