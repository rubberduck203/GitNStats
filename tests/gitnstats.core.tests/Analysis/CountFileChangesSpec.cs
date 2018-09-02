using System;
using System.Collections.Generic;
using System.Linq;

using GitNStats.Core.Tests;
using LibGit2Sharp;
using Xunit;
using Moq;

using static GitNStats.Core.Analysis;
using static GitNStats.Core.Tests.Fakes;

namespace GitNStats.Tests.Analysis
{
    public class CountFileChangesSpec
    {   
        [Fact]
        public void OneEntryIsCounted()
        {
            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/file").Object),
            };
            
            Assert.Equal(1, CountFileChanges(diffs).Single().Count);
        }
        
        [Fact]
        public void TwoEntriesForSameFileAreCounted()
        {
            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/file").Object),
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/file").Object),
            };
            
            Assert.Equal(2, CountFileChanges(diffs).Single().Count);
        }

        [Fact]
        public void TwoEntriesForTwoDifferentFilesAreCountedSeparately()
        {
            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/fileA").Object),
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/fileB").Object),
                (Fakes.Commit().Object, Fakes.TreeEntryChanges("path/to/fileB").Object),
            };
            
            Assert.Equal(1, CountFileChanges(diffs).Single(d => d.Path == "path/to/fileA").Count);
            Assert.Equal(2, CountFileChanges(diffs).Single(d => d.Path == "path/to/fileB").Count);
        }

        [Fact]
        public void RenamedFileHasHistory()
        {
            var fileA = Fakes.TreeEntryChanges("fileA");
            var fileB = Fakes.TreeEntryChanges("fileB");
            fileB.SetupGet(d => d.Status).Returns(ChangeKind.Renamed);
            fileB.SetupGet(d => d.OldPath).Returns(fileA.Object.Path);

            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                (Fakes.Commit().Object, fileA.Object),
                (Fakes.Commit().Object, fileB.Object)
            };

            var pathCounts = CountFileChanges(diffs);
            Assert.Equal("fileB", pathCounts.Single().Path);
            Assert.Equal(2, pathCounts.Single().Count);
        }
    }
}