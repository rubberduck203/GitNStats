using System;
using System.Linq;
using System.Threading.Tasks;

using GitNStats;
using GitNStats.Core;
using GitNStats.Core.Tests;
using LibGit2Sharp;
using Moq;
using Xunit;

namespace gitnstats.test
{
    public class ControllerTests
    {
        private readonly Mock<View> _view;
        private readonly Mock<FileSystem> _fileSystem;
        
        private readonly Mock<AsyncVisitor> _asyncVisitor;

        private readonly Mock<Branch> _currentBranch;
        private readonly Mock<Branch> _otherBranch;
        
        private readonly Controller _controller;

        public ControllerTests()
        {
            _view = new Mock<View>();
            _fileSystem = new Mock<FileSystem>();
            _fileSystem.Setup(fs => fs.CurrentDirectory()).Returns("/path/to/current/directory");
            
            _currentBranch = new Mock<Branch>();
            _currentBranch.Setup(b => b.FriendlyName).Returns("master");
            
            _otherBranch = new Mock<Branch>();
            _otherBranch.Setup(b => b.FriendlyName).Returns("next");
            
            var branches = new Mock<BranchCollection>();
            branches.Setup(b => b[_currentBranch.Object.FriendlyName]).Returns(_currentBranch.Object);
            branches.Setup(b => b[_otherBranch.Object.FriendlyName]).Returns(_otherBranch.Object);
            
            var repository = new Mock<IRepository>();
            repository.SetupGet(r => r.Head).Returns(_currentBranch.Object);            
            repository.Setup(r => r.Branches).Returns(branches.Object);

            IRepository RepositoryFactory(string repoPath) => repository.Object;

            _asyncVisitor = new Mock<AsyncVisitor>();
            AsyncVisitor AsyncVisitorFactory(IRepository repo) => _asyncVisitor.Object;

            _controller = new Controller(_view.Object, _fileSystem.Object, RepositoryFactory, AsyncVisitorFactory);
        }
        
        [Fact]
        public async Task WhenNoRepositorySpecified_AssumesCurrentDirectory()
        {
            var options = new Options();
            
            var result = await _controller.RunAnalysis(options);
            
            _fileSystem.Verify(fs => fs.CurrentDirectory(), Times.Once);
            Assert.Equal(Result.Success, result);
        }

        [Fact]
        public async Task WhenRepositoryFieldIsSpecified_UseIt()
        {
            var options = new Options() {RepositoryPath = "path/to/repo"};
            var result = await _controller.RunAnalysis(options);
            
            _fileSystem.Verify(fs => fs.CurrentDirectory(), Times.Never);
            Assert.Equal(Result.Success, result);
        }

        [Fact]
        public async Task WhenNoBranchIsSpecified_CurrentBranchNameIsDisplayed()
        {
            var options = new Options();
            var result = await _controller.RunAnalysis(options);
            
            _view.Verify(v => v.DisplayRepositoryInfo(_fileSystem.Object.CurrentDirectory(), _currentBranch.Object), Times.Once);
        }

        [Fact]
        public async Task WhenBranchIsSpecified_ThatBranchNameIsDisplayed()
        {
            var options = new Options() { BranchName = _otherBranch.Object.FriendlyName };
            await _controller.RunAnalysis(options);
            
            _view.Verify(v => v.DisplayRepositoryInfo(_fileSystem.Object.CurrentDirectory(), _otherBranch.Object), Times.Once);
        }

        [Fact]
        public async Task WhenSpecifiedBranchDoesNotExist_DisplayError()
        {
            var options = new Options() { BranchName = "applesauce" };
            await _controller.RunAnalysis(options);
            
            _view.Verify(v => v.DisplayError("Invalid branch: applesauce"), Times.Once);
        }
        
        [Fact]
        public async Task WhenSpecifiedBranchDoesNotExist_ReturnsFailure()
        {
            var options = new Options() { BranchName = "applesauce" };
            var result = await _controller.RunAnalysis(options);
            
            Assert.Equal(Result.Failure, result);
        }

        [Fact]
        public async Task WhenRepositoryPathIsNotARepository_DisplayError()
        {
            IRepository RepositoryFactory(string repoPath) => throw new RepositoryNotFoundException();
            
            var controller = new Controller(_view.Object, _fileSystem.Object, RepositoryFactory, (repo) => null);

            await controller.RunAnalysis(new Options());
            
            _view.Verify(v => v.DisplayError($"{_fileSystem.Object.CurrentDirectory()} is not a git repository."));
        }
        
        [Fact]
        public async Task WhenRepositoryPathIsNotARepository_ReturnFailure()
        {
            IRepository RepositoryFactory(string repoPath) => throw new RepositoryNotFoundException();
            var controller = new Controller(_view.Object, _fileSystem.Object, RepositoryFactory, (repo) => null);

            var result = await controller.RunAnalysis(new Options());
            
            Assert.Equal(Result.Failure, result);
        }

        [Fact]
        public async Task DisplaysPathCounts()
        {
            var diffs = Enumerable.Empty<(Commit, TreeEntryChanges)>();

            _asyncVisitor.Setup(v => v.Walk(It.IsAny<Commit>()))
                .Returns(Task.FromResult(diffs));

            var pathCounts = Analysis.CountFileChanges(diffs);

            var result = await _controller.RunAnalysis(new Options());
            
            _view.Verify(v => v.DisplayPathCounts(pathCounts));
            Assert.Equal(Result.Success, result);
        }

        [Fact]
        public async Task WhenDateFilterIsPresent_DisplayFilteredResults()
        {
            var diffs = Fakes.Diffs(Fakes.Commit().WithAuthor(DateTimeOffset.Parse("2017-06-21 13:30 -4:00")));
            
            _asyncVisitor.Setup(v => v.Walk(It.IsAny<Commit>()))
                .Returns(Task.FromResult(diffs));
            
            var result = await _controller.RunAnalysis(new Options(){DateFilter = DateTime.Parse("2017-07-21")});

            var pathCounts = Analysis.CountFileChanges(Enumerable.Empty<(Commit, TreeEntryChanges)>());
            _view.Verify(v => v.DisplayPathCounts(pathCounts));
            Assert.Equal(Result.Success, result);            
        }
    }
}