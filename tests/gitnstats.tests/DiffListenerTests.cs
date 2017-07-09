using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GitNStats;
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
            
            var expected = new Mock<TreeChanges>();
            var expectedAsEnumerable = expected.As<IEnumerable<TreeEntryChanges>>();
            expected.Setup(e => e.GetEnumerator()).Returns(treeEntryChanges.GetEnumerator());
            expected.As<IEnumerable>().Setup(e => e.GetEnumerator()).Returns(treeEntryChanges.GetEnumerator());
            expectedAsEnumerable.Setup(e => e.GetEnumerator()).Returns(treeEntryChanges.GetEnumerator());
            
            var diff = new Mock<Diff>();
            diff.Setup(d => d.Compare<TreeChanges>(It.IsAny<Tree>(), It.IsAny<Tree>()))
                .Returns(expected.Object);
            var repo = new Mock<IRepository>();
            repo.Setup(r => r.Diff).Returns(diff.Object);
            
            var commit = Fakes.Commit().WithParents().Object;
 
            //act
            var listener = new DiffListener(repo.Object);
            listener.OnCommitVisited(new CommitVisitor(), commit);
            
            //assert
            Assert.Equal(treeEntryChanges, listener.Diffs.ToList().OrderBy(x => x.Path));
        }
    }
}