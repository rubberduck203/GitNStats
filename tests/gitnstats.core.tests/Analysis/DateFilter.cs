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
        static TimeSpan AdtOffset = new TimeSpan(-3,0,0);
        static TimeSpan EstOffset = new TimeSpan(-4,0,0);
        static TimeSpan CstOffset = new TimeSpan(-6,0,0);

        static DateTime TestTime = new DateTime(2017,6,21,13,30,0);

        [Fact]
        public void LaterThanFilter_ReturnsTrue()
        {
            var commitTime = new DateTimeOffset(TestTime, EstOffset) + TimeSpan.FromDays(1);
            var commit = Commit().WithAuthor(commitTime);
            var predicate = OnOrAfter(TestTime.Date);
            Assert.True(predicate(Diff(commit)));
        }
        
        [Fact]
        public void PriorToFilter_ReturnsFalse()
        {
            var commitTime = new DateTimeOffset(TestTime, EstOffset) - TimeSpan.FromDays(1);
            var commit = Commit().WithAuthor(commitTime);
            var predicate = OnOrAfter(TestTime.Date);
            Assert.False(predicate(Diff(commit)));
        }

        [Fact]
        public void GivenTimeInESTAndCommitWasInADT_ReturnsFalse()
        {
            var commit = Commit().WithAuthor(new DateTimeOffset(TestTime, AdtOffset));
            var predicate = OnOrAfter(new DateTimeOffset(TestTime, EstOffset).LocalDateTime);
            Assert.False(predicate(Diff(commit)));
        }

        [Fact]
        public void WhenEqualTo_ReturnTrue()
        {
            var commitTime = new DateTimeOffset(TestTime, EstOffset);
            var commit = Commit().WithAuthor(commitTime);
            var predicate = OnOrAfter(commitTime.LocalDateTime);
            Assert.True(predicate(Diff(commit)));
        }
        
        [Fact]
        public void GivenTimeInESTAndCommitWasCST_ReturnsTrue()
        {
            var commit = Commit().WithAuthor(new DateTimeOffset(TestTime, CstOffset));
            var predicate = OnOrAfter(new DateTimeOffset(TestTime, EstOffset).LocalDateTime);
            Assert.True(predicate(Diff(commit)));
        }
    }
}