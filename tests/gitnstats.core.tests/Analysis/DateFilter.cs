using System;
using GitNStats.Core.Tests;
using LibGit2Sharp;
using Xunit;

using static GitNStats.Core.Analysis;
using static GitNStats.Core.Tests.Fakes;

namespace GitNStats.Tests.Analysis
{
    public class DateFilter
    {   
        [Fact]
        public void LaterThanFilter_ReturnsTrue()
        {
            var commit = Commit().WithAuthor(DateTimeOffset.Parse("2017-06-22 13:30 -4:00"));
            var predicate = OnOrAfter(new DateTime(2017, 6, 21));
            Assert.True(predicate(Diff(commit)));
        }
        
        [Fact]
        public void PriorToFilter_ReturnsFalse()
        {
            var commit = Commit().WithAuthor(DateTimeOffset.Parse("2017-06-20 13:30 -4:00"));
            var predicate = OnOrAfter(new DateTime(2017, 6, 21));
            Assert.False(predicate(Diff(commit)));
        }

        [Fact]
        public void GivenTimeInESTAndCommitWasInADT_ReturnsFalse()
        {
            var datetime = new DateTime(2017,6,21,13,30,0);
            var adtOffset = new TimeSpan(-3,0,0);
            var estOffset = new TimeSpan(-4,0,0);
            var commit = Commit().WithAuthor(new DateTimeOffset(datetime, adtOffset));
            var predicate = OnOrAfter(new DateTimeOffset(datetime,estOffset).LocalDateTime);
            Assert.False(predicate(Diff(commit)));
        }

        [Fact]
        public void WhenEqualTo_ReturnTrue()
        {
            var commit = Commit().WithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -4:00"));
            var predicate = OnOrAfter(new DateTime(2017, 6, 21, 13, 30, 0, DateTimeKind.Local));
            Assert.True(predicate(Diff(commit)));
        }
        
        [Fact]
        public void GivenTimeInESTAndCommitWasCST_ReturnsTrue()
        {
            var commit = Commit().WithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -6:00"));
            var predicate = OnOrAfter(new DateTime(2017, 6, 21, 13, 30, 0, DateTimeKind.Local));
            Assert.True(predicate(Diff(commit)));
        }
    }
}