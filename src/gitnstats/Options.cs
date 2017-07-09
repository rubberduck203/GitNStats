using System;
using CommandLine;

namespace GitNStats
{
    public class Options
    {
        private const string RepoPathHelpText = "Path to the git repository to be analyzed. Defaults to the current working directory.";
        
        [Value(1, HelpText = RepoPathHelpText, MetaName = "FilePath")]
        public string RepositoryPath { get; set; }
    }
}