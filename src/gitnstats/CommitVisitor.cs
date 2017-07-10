using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace GitNStats
{
    /// <summary>
    /// Walks the commit graph back to the beginning of time.
    /// Guaranteed to only visit a commit once.
    /// </summary>
    public class CommitVisitor : Visitor
    {
        /// <summary>
        /// Walk the graph from this commit back.
        /// </summary>
        /// <param name="commit">The commit to start at.</param>
        public override void Walk(Commit commit)
        {
            Walk(commit, new HashSet<string>());
        }

        private void Walk(Commit commit, ISet<string> visited)
        {
            if (!visited.Add(commit.Sha))
            {
                // Exit so we don't walk the same path multiple times.
                return;
            }

            visited.Add(commit.Sha);
            OnVisited(this, commit);
            
            foreach(var parent in commit.Parents)
            {
                Walk(parent, visited);
            }
        }
    }
}