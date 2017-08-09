using System.Collections.Generic;
using LibGit2Sharp;
using static GitNStats.Core.Tooling;

namespace GitNStats.Core
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
            //WithStopWatch(() => Walk(commit, new HashSet<string>()), "Total Time Walking Graph: {0}");
        }

        private void Walk(Commit commit, ISet<string> visited)
        {
            if (!visited.Add(commit.Sha))
            {
                // Exit so we don't walk the same path multiple times.
                return;
            }

            OnVisited(this, commit);

            foreach (var parent in commit.Parents)
            {
                Walk(parent, visited);
            }
        }
    }
}