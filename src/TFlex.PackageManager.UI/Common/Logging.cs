using System;
using System.IO;

namespace TFlex.PackageManager.UI.Common
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
        internal void PrintHelper()
        {
            sw.WriteLine("LOGGING INFO");
            sw.WriteLine("======================================================");
            sw.WriteLine("Action enums for Document:");
            sw.WriteLine("0-0 : Opened");
            sw.WriteLine("0-1 : Allocated new Document from Prototype");
            sw.WriteLine("0-2 : Regenerating the model");
            sw.WriteLine("0-3 : Saved");
            sw.WriteLine("0-4 : Saved As");
            sw.WriteLine("0-5 : Canceled Changes");
            sw.WriteLine("0-6 : Closed");
            sw.WriteLine("0-7 : Removed");
            sw.WriteLine("Action enums for Links:");
            sw.WriteLine("1-0 : Read");
            sw.WriteLine("1-1 : Replaced");
            sw.WriteLine("Action enums for Pages:");
            sw.WriteLine("2-0 : Read");
            sw.WriteLine("2-1 : Write");
            sw.WriteLine("Action enums for Projections:");
            sw.WriteLine("3-0 : Read");
            sw.WriteLine("3-1 : Write");
            sw.WriteLine("Action enums for Variables:");
            sw.WriteLine("4-1 : Add");
            sw.WriteLine("4-2 : Edit");
            sw.WriteLine("4-3 : Rename");
            sw.WriteLine("4-4 : Remove");
            sw.WriteLine("======================================================");
            sw.WriteLine("######## L [DD HH:MM:SS:FFFFFF] A-E LINE DESCRIPTION");
            sw.WriteLine("======================================================");
        }

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