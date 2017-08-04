using LibGit2Sharp;

namespace GitNStats.Core
{
    /// <summary>
    /// Listens for a <see cref="Visitor"/> to raise the <see cref="Visitor.Visited"/> event.
    /// </summary>
    public interface Listener
    {
        /// <summary>
        /// To be executed whenever a visitor vistis a commit.
        /// </summary>
        /// <param name="visitor">The <see cref="Visitor"/> that raised the <see cref="Visitor.Visited"/> event.</param>
        /// <param name="visited">The <see cref="Commit"/> currently being visited.</param>
        void OnCommitVisited(Visitor visitor, Commit visited);
    }
}