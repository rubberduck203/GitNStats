using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitNStats.Core;
using LibGit2Sharp;
using Xunit;
using Moq;

namespace GitNStats.Tests.Visitor
{
    public class DiffWalkerTests
    {
        [Fact]
        public async Task WhenVisitorVisitsOneCommit_OneDiffIsReturned()
        {
            var commit = Fakes.Commit().Object;
            
            var visitor = new Mock<Core.Visitor>();
            visitor.Setup(v => v.Walk(commit))
                .Raises(v => v.Visited += null, visitor.Object, commit);
            
            var listener = new Mock<IDiffListener>();
            listener.Setup(l => l.OnCommitVisited(It.IsAny<Core.Visitor>(), It.IsAny<Commit>()))
                .Callback(()=> 
                    listener.SetupGet(l => l.Diffs)
                        .Returns(new List<(Commit, TreeEntryChanges)>()
                        {
                            (commit, new Mock<TreeEntryChanges>().Object)
                        })
                    );
            
            var walker = new DiffCollector(visitor.Object, listener.Object);
            var result = await walker.Walk(commit);
            
            Assert.Equal(commit.Sha, result.First().Commit.Sha);
        }
    }
}