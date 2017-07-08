using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GitNStats;
using LibGit2Sharp;

using Moq;
using Xunit;

namespace GitNStats.Tests.TreeExtensions
{
    public class Flatten
    {
        [Fact]
        public void EmptyTree_EmptyResult()
        {
            var input = new List<TreeEntry>();
            var tree = new Mock<Tree>(MockBehavior.Strict);

            tree.Setup(t => t.GetEnumerator()).Returns(input.GetEnumerator());

            Assert.Empty(tree.Object.Flatten());
        }

        [Fact]
        public void OneBlob_OneBlob()
        {
            var treeEntry = new Mock<TreeEntry>();
            treeEntry.Setup(te => te.TargetType).Returns(TreeEntryTargetType.Blob);

            var input = new List<TreeEntry>()
            {
               treeEntry.Object
            };

            var tree = new Mock<Tree>(MockBehavior.Strict);
            SetupEnumerator(tree, input);

            //must materialize the IEnumerable because the IEnumerator doesn't get reset.
            var actual = tree.Object.Flatten().ToList(); 

            Assert.Contains(treeEntry.Object, actual);
            Assert.NotEmpty(actual);
            Assert.Equal(1, actual.Count());
        }

        private void SetupEnumerator(Mock<Tree> tree, List<TreeEntry> input)
        {
            tree.Setup(t => t.GetEnumerator()).Returns(input.GetEnumerator());
            tree.As<IEnumerable>().Setup(t => t.GetEnumerator()).Returns(input.GetEnumerator());
            tree.As<IEnumerable<TreeEntry>>().Setup(t => t.GetEnumerator()).Returns(input.GetEnumerator());
        }
    }
}
