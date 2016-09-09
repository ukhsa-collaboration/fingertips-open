using System;
using System.IO;
using System.Text;
using StackExchange.Profiling;

namespace ServicesWeb.Helpers
{
    public class MiniProfilerWriter
    {
        public void Write(MiniProfiler miniProfiler)
        {
            if (miniProfiler != null && miniProfiler.DurationMilliseconds > 50)
            {
                StringBuilder sb = new StringBuilder();

                // Write step times
                if (miniProfiler.Root.HasChildren)
                {
                    var children = miniProfiler.Root.Children;
                    foreach (var child in children)
                    {
                        sb.AppendLine(child.Name + " " + child.DurationMilliseconds);
                    }
                }

                // Write overall request time
                sb.AppendLine(string.Format("{0} {1}\n",
                    miniProfiler.DurationMilliseconds,
                    miniProfiler.Root));

                // Write to file
                try
                {
                    File.AppendAllText(@"c:\temp\out.txt", sb.ToString());
                }
                catch (Exception)
                {
                }
            }
        } 
    }
}