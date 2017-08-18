using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            //Walk(commit, new HashSet<string>());
            WithStopWatch(() => Walk(commit, new HashSet<string>()), "Total Time Walking Graph: {0}");
        }

        private Object padlock = new Object();
        private void Walk(Commit commit, ISet<string> visitedCommits)
        {
            // It's not safe to concurrently write to the Set.
            // If two threads hit this at the same time we could visit the same commit twice.
            bool added;
            lock(padlock)
            {
                added = visitedCommits.Add(commit.Sha);
            }

            // If we weren't successful in adding the commit, we've already been here.
            if (!added) return;

            OnVisited(this, commit);

            Parallel.ForEach(commit.Parents, parent =>
            {
                Walk(parent, visitedCommits);
            });
        }
    }
}