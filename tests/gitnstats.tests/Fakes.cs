using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Moq;

namespace GitNStats.Tests
{
    public static class Fakes
    {
        public static Mock<Commit> Commit() 
        {
            var guid = Guid.NewGuid().ToString();
            var commit = new Mock<Commit>();
            commit.Setup(c => c.Sha).Returns(guid);
            commit.WithParents(Enumerable.Empty<Commit>());
            return commit;
        }
        
        public static Mock<Commit> WithParents(this Mock<Commit> commit)
        {
            commit.WithParents(new List<Commit>() {Commit().Object, Commit().Object});
            return commit;
        }

        public static Mock<Commit> WithParents(this Mock<Commit> commit, IEnumerable<Commit> parents)
        {
            commit.Setup(c => c.Parents).Returns(parents);
            return commit;
        }
    }
}