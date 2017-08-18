using System.Collections.Generic;
using System.Linq;
using GitNStats.Core;
using GitNStats.Core.Tests;
using LibGit2Sharp;
using Moq;
using Xunit;

namespace GitNStats.Tests
{
    public class DiffListenerTests
    {
        [Fact]
        public void WhenCommitIsVisited_DiffWithItsParentIsStored()
        {
            //arrange
            var treeChangeA = new Mock<TreeEntryChanges>();
                treeChangeA.Setup(t => t.Path).Returns("a");
            var treeChangeB = new Mock<TreeEntryChanges>();
                treeChangeB.Setup(t => t.Path).Returns("b");
            var treeEntryChanges = new List<TreeEntryChanges>()
            {
                treeChangeA.Object,
                treeChangeB.Object
            };
            
            var expected = Fakes.TreeChanges(treeEntryChanges);

            var diff = new Mock<Diff>();
            diff.Setup(d => d.Compare<TreeChanges>(It.IsAny<Tree>(), It.IsAny<Tree>()))
                .Returns(expected.Object);
            var repo = new Mock<IRepository>();
            repo.Setup(r => r.Diff)
                .Returns(diff.Object);
            
            var commit = Fakes.Commit().WithParents().Object;
 
            //act
            var listener = new DiffListener(repo.Object);
            listener.OnCommitVisited(new CommitVisitor(), commit);
            
            var actual = listener.Diffs
                .Select(d => d.Diff)
                .OrderBy(change => change.Path)
                .ToList();

            //assert
            Assert.Equal(treeEntryChanges, actual);
            Assert.Equal(commit, listener.Diffs.Select(d => d.Commit).First());
        }
    }
}