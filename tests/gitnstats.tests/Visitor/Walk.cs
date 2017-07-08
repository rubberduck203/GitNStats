using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GitNStats;
using LibGit2Sharp;

using Moq;
using Xunit;

namespace GitNStats.Tests.Visitor
{
    public class Walk
    {
        [Fact]
        public void NoParents_OnlyVisitsCurrentCommit()
        {
            var parents = new List<Commit>();
            var commit = SetupCommit(parents);
            
            CommitVisitor visitor = new CommitVisitor();

            int visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;
            visitor.Walk(commit.Object);

            Assert.Equal(1, visitedCount);
        }

        [Fact]
        public void OneParent_VisitsCurrentCommitAndParent()
        {
            var parents = new List<Commit>() 
            {
                SetupCommit().Object
            };
            var commit = SetupCommit(parents);
            
            CommitVisitor visitor = new CommitVisitor();

            int visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;

            visitor.Walk(commit.Object);

            Assert.Equal(2, visitedCount);
        }

        [Fact]
        public void TwoParents_VisitsCurrentCommitAndBothParents()
        {
            var parents = new List<Commit>() 
            {
                SetupCommit().Object,
                SetupCommit().Object
            };
            var commit = new Mock<Commit>();
            commit.Setup(c => c.Parents).Returns(parents);
            
            CommitVisitor visitor = new CommitVisitor();

            int visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;

            visitor.Walk(commit.Object);

            Assert.Equal(3, visitedCount);
        }

        [Fact]
        public void ParentsShareAParent_OnlyVisitGrandParentOnce()
        {
            var grandParent = SetupCommit(Enumerable.Empty<Commit>());
            var grandParents = new List<Commit>() { grandParent.Object };

            var parent1 = SetupCommit(grandParents);
            var parent2 = SetupCommit(grandParents);

            var parents = new List<Commit>() 
            {
                parent1.Object,
                parent2.Object
            };

            var commit = SetupCommit(parents);
            
            CommitVisitor visitor = new CommitVisitor();

            int visitedCount = 0;
            int grandParentVisitedCount = 0;
            visitor.Visited += (sender, visited) => {
                visitedCount++;
                if (visited.Sha == grandParent.Object.Sha)
                {
                    grandParentVisitedCount++;
                }
            };

            visitor.Walk(commit.Object);

            Assert.Equal(4, visitedCount);
            Assert.Equal(1, grandParentVisitedCount);
        }

        private Mock<Commit> SetupCommit() 
        {
            return SetupCommit(Enumerable.Empty<Commit>());
        }
        private Mock<Commit> SetupCommit(IEnumerable<Commit> parents)
        {
            string guid = Guid.NewGuid().ToString();
            var commit = new Mock<Commit>();
            commit.Setup(c => c.Sha).Returns(guid);
            commit.Setup(c => c.Parents).Returns(parents);

            return commit;
        }
    }
}