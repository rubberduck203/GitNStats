using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using GitNStats;
using LibGit2Sharp;

using Moq;
using Xunit;

namespace GitNStats.Tests.CommitExtensions
{
    public class Walk
    {
        [Fact]
        public void GivenACommitHasNoParents_PerformsActionOnce()
        {
            var commits = new List<Commit>()
            {
                new Mock<Commit>().Object
            };

            int counter = 0;
            commits.Walk((_) => counter++);

            Assert.Equal(1, counter);
        }

        [Fact]
        public void GivenACommitHasAParent_PerformsActionTwice()
        {
            var commit = new Mock<Commit>();
            commit.Setup(c => c.Parents)
                .Returns(new List<Commit>(){new Mock<Commit>().Object});

            var commits = new List<Commit>(){ commit.Object };

            int counter = 0;
            commits.Walk((_) => counter++);

            Assert.Equal(2, counter);
        }

        [Fact]
        public void AcceptsARepository()
        {
            using(var repo = new Repository())
            {
                var commit = new Mock<Commit>();
                commit.Setup(c => c.Parents)
                    .Returns(new List<Commit>(){new Mock<Commit>().Object});

                var commits = new List<Commit>(){ commit.Object };

                int counter = 0;
                commits.Walk(repo, (r,c) => counter++);

                Assert.Equal(2, counter);
            }
        }

        [Fact]
        public void CanWalkASingleCommit()
        {
            using(var repo = new Repository())
            {
                var commit = new Mock<Commit>();
                commit.Setup(c => c.Parents)
                    .Returns(new List<Commit>(){new Mock<Commit>().Object});

                var commits = new List<Commit>(){ commit.Object };

                int counter = 0;
                commit.Object.Walk(repo, (r,c) => counter++);

                Assert.Equal(2, counter);
            }
        }
    }
}