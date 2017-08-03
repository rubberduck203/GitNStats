using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace GitNStats
{
    public class Options
    {
        private const string RepoPathHelpText = "Path to the git repository to be analyzed. Defaults to the current working directory.";
        
        [Value(1, HelpText = RepoPathHelpText, MetaName = "FilePath")]
        public string RepositoryPath { get; set; }
        
        [Option('b', "branch", HelpText = "Defaults to the currently active branch.")]
        public string BranchName { get; set; }
        
        [Option('d', "date-filter", HelpText = "Get commits on or after this date. Defaults to no filter.")]
        public DateTime? DateFilter { get; set; }
        
        [Usage]
        public static IEnumerable<Example> Examples 
        {
            get 
            {
                yield return new Example("Run on current directory", new Options());
                yield return new Example("Run on specific repository", new Options() { RepositoryPath = "/Users/rubberduck/src/repository"});   
                yield return new Example("Run on specific branch", new Options(){ BranchName = "develop" });
                yield return new Example("Specify date filter", new Options(){DateFilter = DateTime.Today});
            }
        }
    }
}