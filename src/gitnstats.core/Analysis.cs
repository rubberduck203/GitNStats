﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitNStats.Core
{
    public record PathCount(string Path, int Count);
    public record CommitDiff(Commit Commit, TreeEntryChanges Diff);

    public delegate bool DiffFilterPredicate(CommitDiff diff);
    
    public static class Analysis
    {
        public static IEnumerable<PathCount> CountFileChanges(IEnumerable<CommitDiff> diffs) =>
            diffs.Aggregate<CommitDiff, Dictionary<string, int>>(
                new Dictionary<string, int>(), //filename, count
                (acc, x) =>
                {
                    /* OldPath == NewPath when file was created or removed,
                        so this it's okay to just always use OldPath */
                    acc[x.Diff.Path] = acc.GetOrDefault(x.Diff.OldPath, 0) + 1;

                    if (x.Diff.Status == ChangeKind.Renamed)
                    {
                        acc.Remove(x.Diff.OldPath);
                    }

                    return acc;
                }
            )
            .Select(x => new PathCount(x.Key, x.Value))
            .OrderByDescending(s => s.Count);

        /// <summary>
        /// Predicate for filtering by date
        /// </summary>
        /// <param name="onOrAfter">Local DateTime</param>
        /// <returns>True if Commit was on or after <paramref name="onOrAfter"/>, otherwise false.</returns>
        public static DiffFilterPredicate OnOrAfter(DateTime onOrAfter) =>
            change => change.Commit.Author.When.ToUniversalTime() >= onOrAfter.ToUniversalTime();
    }

    static class DictionaryExtensions
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue)
        where K : notnull =>
            dictionary.TryGetValue(key, out V value) ? value : defaultValue;
    }
}