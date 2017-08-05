using LibGit2Sharp;
using CommandLine;
using GitNStats.Core;

namespace GitNStats
{
    static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(options =>
                {
                    IRepository RepositoryFactory(string repoPath) => new Repository(repoPath);
                    AsyncVisitor AsyncVisitorFactory(IRepository repo) => new DiffCollector(new CommitVisitor(), new DiffListener(repo));
                    
                    var controller = new Controller(new CliView(), new FileSystem(), RepositoryFactory, AsyncVisitorFactory);
                    
                    return (int)controller.RunAnalysis(options).GetAwaiter().GetResult();
                }, _ => (int)Result.Failure);
        }
    }
}