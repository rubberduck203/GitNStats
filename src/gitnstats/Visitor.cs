using LibGit2Sharp;

namespace GitNStats
{
    public abstract class Visitor
    {
        public delegate void VisitedHandler(CommitVisitor visitor, Commit commit);
        public event VisitedHandler Visited;
        
        public abstract void Walk(Commit commit);

        protected virtual void OnVisited(CommitVisitor visitor, Commit commit)
        {
            Visited?.Invoke(visitor, commit);
        }
    }
}