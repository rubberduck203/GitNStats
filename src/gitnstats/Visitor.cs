using LibGit2Sharp;

namespace GitNStats
{
    public abstract class Visitor
    {
        public delegate void VisitedHandler(Visitor visitor, Commit commit);
        public virtual event VisitedHandler Visited;
        
        public abstract void Walk(Commit commit);

        protected virtual void OnVisited(Visitor visitor, Commit commit)
        {
            Visited?.Invoke(visitor, commit);
        }
    }
}