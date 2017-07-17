using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using LibGit2Sharp;

namespace GitNStats
{
    /// <summary>
    /// When a Commit is visited, compares that commit to it's parents 
    /// and stores the resulting TreeEntryChanges in the <see cref="Diffs"/> property.
    /// </summary>
    public class DiffListener : Listener
    {
        private readonly IRepository _repository;
        private readonly ConcurrentBag<(Commit, TreeEntryChanges)> _diffs = new ConcurrentBag<(Commit, TreeEntryChanges)>();
        
        /// <summary>
        /// The diff cache. 
        /// Clients should wait until the <see cref="Visitor"/> is done walking the graph before accessing.
        /// </summary>
        public IEnumerable<(Commit, TreeEntryChanges)> Diffs => _diffs;

        public DiffListener(IRepository repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// Compares the <paramref name="visited"/> commit to it's parents and caches the diffs in <see cref="Diffs"/>.
        /// </summary>
        /// <param name="visitor">The <see cref="Visitor"/> that raised the <see cref="Visitor.Visited"/> event.</param>
        /// <param name="visited">The <see cref="Commit"/> currently being visited.</param>
        public void OnCommitVisited(Visitor visitor, Commit visited)
        {
            foreach (var parent in visited.Parents)
            {
                var diff = _repository.Diff.Compare<TreeChanges>(parent.Tree, visited.Tree);
               
                foreach (var changed in diff)
                {
                    _diffs.Add((visited, changed));
                }
            }
        }
    }
}