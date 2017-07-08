using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace GitNStats
{
    public class CommitVisitor
    {
        public event EventHandler<Commit> Visited;
        public void Walk(Commit commit)
        {
            Walk(commit, new HashSet<string>());
        }

        private void Walk(Commit commit, ISet<string> visited)
        {
            if (visited.Contains(commit.Sha))
            {
                // Exit so we don't walk the same path multiple times.
                return;
            }

            visited.Add(commit.Sha);

            Visited?.Invoke(this, commit);
            foreach(var parent in commit.Parents)
            {
                Walk(parent, visited);
            }
        }
    }
}