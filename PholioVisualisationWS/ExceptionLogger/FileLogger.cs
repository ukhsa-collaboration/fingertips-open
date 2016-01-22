using System;
using System.IO;
using System.Text;

namespace PholioVisualisation.ExceptionLogging
{
    /// <summary>
    ///     Simple logger for writing to a text file.
    /// </summary>
    public class FileLogger
    {
        private readonly string path;

        public FileLogger(string path)
        {
            this.path = path;
        }

        public void WriteException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(new String('#', 40));
            sb.AppendLine(DateTime.Now.ToString());

            if (ex != null)
            {
                while (ex != null)
                {
                    AppendException(ex, sb);
                    ex = ex.InnerException;
                }
            }
            else
            {
                sb.AppendLine("NULL exception");
            }
            File.AppendAllText(path, sb.ToString());
        }

        public void WriteLine(string line)
        {
            File.AppendAllText(path, (line ?? string.Empty) + Environment.NewLine);
        }

        private static void AppendException(Exception ex, StringBuilder sb)
        {
            sb.AppendLine("MESSAGE: " + ex.Message);
            sb.AppendLine("TYPE:    " + ex.GetType().Name);
            sb.AppendLine("STACK:   " + ex.StackTrace ?? string.Empty);
        }
    }
}