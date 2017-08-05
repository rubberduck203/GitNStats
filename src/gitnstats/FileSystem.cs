using System;
using System.IO;

namespace GitNStats
{
    /// <summary>
    /// Provides a mockable file system abstraction on top of System.IO
    /// </summary>
    public class FileSystem
    {
        /// <summary>
        /// Gets the current working directory of the application.
        /// </summary>
        public virtual string CurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        } 
    }
}