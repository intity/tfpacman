using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace TFlex.PackageManager.Common
{
    /// <summary>
    /// The LogFile class to output processing data.
    /// </summary>
    internal class LogFile
    {
        #region private fields
        private readonly Options options;
        private readonly List<string> contents;
        private string logFile;
        #endregion

        public LogFile(Options options)
        {
            this.options = options;
            contents     = new List<string>();
        }

        #region internal methods
        /// <summary>
        /// Create log file the documents processing.
        /// </summary>
        /// <param name="targetDirectory"></param>
        internal void CreateLogFile(string targetDirectory)
        {
            if (!options.EnableLogFile)
                return;

            logFile = Path.Combine(targetDirectory, Resource.LOG_FILE);

            if (contents.Count > 0)
                contents.Clear();

            if (File.Exists(logFile))
                File.Delete(logFile);

            File.AppendAllLines(logFile, contents);
        }

        /// <summary>
        /// Append new line to contents.
        /// </summary>
        /// <param name="value"></param>
        internal void AppendLine(string value)
        {
            if (!options.EnableLogFile)
                return;

            contents.Add(value);
        }

        /// <summary>
        /// Set contents to log file.
        /// </summary>
        internal void SetContentsToLog()
        {
            if (!options.EnableLogFile)
                return;

            File.AppendAllLines(logFile, contents);
        }

        /// <summary>
        /// Open log file.
        /// </summary>
        internal void OpenLogFile()
        {
            if (!options.EnableLogFile)
                return;

            if (File.Exists(logFile))
                Process.Start("notepad.exe", logFile);
        }
        #endregion
    }
}