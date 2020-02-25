using System;
using System.IO;

namespace TFlex.PackageManager.Common
{
    internal enum LogLevel
    {
        INFO,
        ERROR
    }

    /// <summary>
    /// The Logging class to output processing data.
    /// </summary>
    internal class Logging
    {
        #region private fields
        private uint counter;
        private readonly StreamWriter sw;
        #endregion

        public Logging(StreamWriter sw)
        {
            this.sw = sw;
        }

        #region internal methods
        /// <summary>
        /// Write new line to log file.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        internal void WriteLine(LogLevel level, string message)
        {
            sw.WriteLine(string.Format("{0:d8} {1} [{2}] {3}", 
                counter, 
                level == LogLevel.INFO ? "I" : "E", 
                DateTime.Now.ToString("dd HH:mm:ss:ffffff"), message));
            counter++;
        }
        #endregion
    }
}