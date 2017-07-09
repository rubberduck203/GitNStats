using System.IO;
using CommandLine;

namespace GitNStats
{
    public class Options
    {
        [Value(1, HelpText = "Path to the git repository to analyze", MetaName = "FilePath", Required = true)]
        public string RepositoryPath { get; set; }
    }
}