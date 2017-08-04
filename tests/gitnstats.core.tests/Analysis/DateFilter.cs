using System;
using System.Collections.Generic;
using System.Linq;
using GitNStats.Core;
using LibGit2Sharp;
using Moq;
using Xunit;

namespace GitNStats.Tests.Analysis
{
    public class DateFilter
    {   
        [Fact]
        public void LaterThanFilter_ReturnsSingleCommit()
        {
            var commit = CommitWithAuthor(DateTimeOffset.Parse("2017-06-22 13:30 -4:00"));
            var diffs = Diffs(commit);

            var actual = diffs.OnOrAfter(new DateTime(2017, 6, 21));
            
            Assert.Equal(1, actual.Count());
        }

        [Fact]
        public void PriorToFilter_ReturnsEmpty()
        {
            var commit = CommitWithAuthor(DateTimeOffset.Parse("2017-06-20 13:30 -4:00"));
            var diffs = Diffs(commit);

            var actual = diffs.OnOrAfter(new DateTime(2017, 6, 21));
            
            Assert.Equal(0, actual.Count());
        }
        
        [Fact]
        public void GivenTimeInESTAndCommitWasInADT_ReturnsCommit()
        {
            var commit = CommitWithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -3:00"));
            var diffs = Diffs(commit);
            
            var actual = diffs.OnOrAfter(new DateTime(2017, 6, 21, 13, 30, 0, DateTimeKind.Local));
            
            Assert.Equal(0, actual.Count());
        }

        [Fact]
        public void WhenEqualTo_ReturnCommit()
        {
            var commit = CommitWithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -4:00"));
            var diffs = Diffs(commit);

            var actual = diffs.OnOrAfter(new DateTime(2017, 6, 21, 13, 30, 0, DateTimeKind.Local));
            
            Assert.Equal(1, actual.Count());
        }
        
        [Fact]
        public void GivenTimeInESTAndCommitWasCST_ReturnsCommit()
        {
            var commit = CommitWithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -6:00"));
            var diffs = Diffs(commit);

            var actual = diffs.OnOrAfter(new DateTime(2017, 6, 21, 13, 30, 0, DateTimeKind.Local));
            
            Assert.Equal(1, actual.Count());
        }
        
        //TODO: Evaluate moving private methods to Fakes
        private static IEnumerable<(Commit, TreeEntryChanges)> Diffs(Mock<Commit> commit)
        {
            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                (commit.Object, Fakes.TreeEntryChanges("path/to/file").Object)
            };
            return diffs;
        }
        
        private Mock<Commit> CommitWithAuthor(DateTimeOffset dateTimeOffset)
        {
            var commit = Fakes.Commit();
            commit.Setup(c => c.Author)
                .Returns(Signature(dateTimeOffset));
            return commit;
        }
        
        private Signature Signature(DateTimeOffset dateTimeOffset)
        {
            return new Signature("rubberduck", "rubberduck@example.com", dateTimeOffset);
        }
    }
}