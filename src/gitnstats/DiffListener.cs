using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using LibGit2Sharp;

namespace GitNStats
{
    public class DiffListener
    {
        private readonly IRepository _repository;
        private readonly ConcurrentBag<TreeEntryChanges> _diffs = new ConcurrentBag<TreeEntryChanges>();
        
        public IEnumerable<TreeEntryChanges> Diffs => _diffs;

        public DiffListener(IRepository repository)
        {
            _repository = repository;
        }
        
        public void OnCommitVisited(CommitVisitor visitor, Commit visited)
        {
            foreach (var parent in visited.Parents)
            {
                var diff = _repository.Diff.Compare<TreeChanges>(parent.Tree, visited.Tree);
               
                foreach (var changed in diff)
                {
                    _diffs.Add(changed);
                }
            }
        }
    }
}