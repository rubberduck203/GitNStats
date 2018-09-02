using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Moq;

namespace GitNStats.Core.Tests
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

        public static Mock<Commit> WithAuthor(this Mock<Commit> commit, Signature author)
        {
            commit.Setup(c => c.Author)
                .Returns(author);

            return commit;
        }

        public static Mock<Commit> WithAuthor(this Mock<Commit> commit, DateTimeOffset commitTime)
        {
            return commit.WithAuthor(Signature(commitTime));
        }
        
        /*
            I don't really like having TreeChanges and Commit in the same file.
            But this does provide a very nice `Fakes.Commit().WithParents()`
            and `Fakes.TreeChanges(somelist)` interface.
            
            I'd love to find a way to keep the interface but separate these.
            I tried using Fake as the namespace, but wasn't happy with the result.
        */

        public static Mock<TreeEntryChanges> TreeEntryChanges(string filePath)
        {
            var treeChanges = new Mock<TreeEntryChanges>();
            treeChanges.Setup(t => t.Path).Returns(filePath);
            treeChanges.Setup(t => t.Status).Returns(ChangeKind.Modified);
            
            return treeChanges;
        }
        
        public static Mock<TreeChanges> TreeChanges(IEnumerable<TreeEntryChanges> treeEntryChanges)
        {
            var treeChanges = new Mock<TreeChanges>();
            // Calling GetEnumerator doesn't actually enumerate the collection.
            // ReSharper disable PossibleMultipleEnumeration
            treeChanges.Setup(e => e.GetEnumerator())
                .Returns(treeEntryChanges.GetEnumerator());
            treeChanges.As<IEnumerable>()
                .Setup(e => e.GetEnumerator())
                .Returns(treeEntryChanges.GetEnumerator());
            treeChanges.As<IEnumerable<TreeEntryChanges>>()
                .Setup(e => e.GetEnumerator())
                .Returns(treeEntryChanges.GetEnumerator());
            // ReSharper restore PossibleMultipleEnumeration
            return treeChanges;
        }

        public static IEnumerable<(Commit, TreeEntryChanges)> Diffs(Mock<Commit> commit)
        {
            var diffs = new List<(Commit, TreeEntryChanges)>()
            {
                Diff(commit)
            };
            return diffs;
        }

        public static (Commit, TreeEntryChanges) Diff(Mock<Commit> commit)
        {
            return (commit.Object, TreeEntryChanges("path/to/file").Object);
        }

        public static Signature Signature(DateTimeOffset dateTimeOffset)
        {
            return new Signature("rubberduck", "rubberduck@example.com", dateTimeOffset);
        }
    }
}