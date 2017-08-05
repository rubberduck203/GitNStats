using System.Collections.Generic;
using GitNStats.Core;
using GitNStats.Core.Tests;
using LibGit2Sharp;
using Xunit;

namespace GitNStats.Tests.Visitor
{
    public class Walk
    {
        [Fact]
        public void NoParents_OnlyVisitsCurrentCommit()
        {
            var commit = Fakes.Commit();
            
            var visitor = new CommitVisitor();

            var visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;
            visitor.Walk(commit.Object);

            Assert.Equal(1, visitedCount);
        }

        [Fact]
        public void OneParent_VisitsCurrentCommitAndParent()
        {
            var commit = Fakes.Commit()
                .WithParents(new List<Commit>() 
                {
                    Fakes.Commit().Object
                });
            
            var visitor = new CommitVisitor();

            var visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;

            visitor.Walk(commit.Object);

            Assert.Equal(2, visitedCount);
        }

        [Fact]
        public void TwoParents_VisitsCurrentCommitAndBothParents()
        {
            var commit = Fakes.Commit()
                .WithParents(new List<Commit>() 
                {
                    Fakes.Commit().Object,
                    Fakes.Commit().Object
                });
            
            var visitor = new CommitVisitor();

            var visitedCount = 0;
            visitor.Visited += (sender, visited) => visitedCount++;

            visitor.Walk(commit.Object);

            Assert.Equal(3, visitedCount);
        }

        [Fact]
        public void ParentsShareAParent_OnlyVisitGrandParentOnce()
        {
            var grandParent = Fakes.Commit();
            var grandParents = new List<Commit>() { grandParent.Object };

            var commit = Fakes.Commit()
                .WithParents(new List<Commit>() 
                {
                    Fakes.Commit().WithParents(grandParents).Object,
                    Fakes.Commit().WithParents(grandParents).Object
                });
            
            var visitor = new CommitVisitor();

            var visitedCount = 0;
            var grandParentVisitedCount = 0;
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

        [Fact]
        public void CanCallWalkMultipleTimes()
        {
            var grandParent = Fakes.Commit();
            var grandParents = new List<Commit>() { grandParent.Object };

            var commit = Fakes.Commit()
                .WithParents(new List<Commit>() 
                {
                    Fakes.Commit().WithParents(grandParents).Object,
                    Fakes.Commit().WithParents(grandParents).Object
                });
            
            var visitor = new CommitVisitor();

            var visitedCount = 0;
            var grandParentVisitedCount = 0;
            visitor.Visited += (sender, visited) => {
                visitedCount++;
                if (visited.Sha == grandParent.Object.Sha)
                {
                    grandParentVisitedCount++;
                }
            };

            visitor.Walk(commit.Object);
            visitor.Walk(commit.Object);

            Assert.Equal(8, visitedCount);
            Assert.Equal(2, grandParentVisitedCount);
        }
    }
}