using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitNStats
{
    public class DiffCollector
    {
        private readonly Visitor _visitor;
        private readonly IDiffListener _listener;

        public DiffCollector(Visitor visitor, IDiffListener listener)
        {
            _visitor = visitor;
            _listener = listener;
        }

        public Task<IEnumerable<(Commit Commit, TreeEntryChanges Diff)>> Walk(Commit commit)
        {
            return Task.Run(() =>
            {
                _visitor.Visited += _listener.OnCommitVisited;
                try
                {
                    _visitor.Walk(commit);
                    return Task.FromResult(_listener.Diffs);
                }
                finally
                {
                    _visitor.Visited -= _listener.OnCommitVisited;
                }
            });
        }
    }
}