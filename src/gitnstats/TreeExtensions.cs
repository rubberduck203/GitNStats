using System;
using LibGit2Sharp;
using System.Collections.Generic;

namespace GitNStats
{
    public static class TreeExtensions
    {
        public static IEnumerable<TreeEntry> Flatten(this Tree tree)
        {
            foreach (var treeEntry in tree)
            {
                switch (treeEntry.TargetType)
                {
                    case TreeEntryTargetType.Blob:
                    case TreeEntryTargetType.GitLink:
                        yield return treeEntry;
                        break;
                    case TreeEntryTargetType.Tree:
                        foreach (var innerEntry in Flatten((Tree)treeEntry.Target))
                        {
                            yield return innerEntry;
                        }
                        break;
                }
            }
        }
    }
}