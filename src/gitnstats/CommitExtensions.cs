using System;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;

namespace GitNStats
{
    public static class CommitExtensions
    {
        public static void Walk(this IEnumerable<Commit> commits, Action<Commit> action)
        {
            foreach(var commit in commits)
            {
                commit.Walk(action);
            }
        }

        public static void Walk(this IEnumerable<Commit> commits, Repository repo, Action<Repository, Commit> action)
        {
            foreach(var commit in commits)
            {
                commit.Walk(repo, action);
            }
        }

        public static void Walk(this Commit commit, Action<Commit> action)
        {
            action(commit);
            commit.Parents.Walk(action);
        }

        public static void Walk(this Commit commit, Repository repo, Action<Repository, Commit> action)
        {
            action(repo, commit);
            commit.Parents.Walk(repo, action);
        }
    }
}