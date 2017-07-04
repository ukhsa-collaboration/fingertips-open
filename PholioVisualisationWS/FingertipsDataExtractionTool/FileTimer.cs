using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;

namespace FingertipsDataExtractionTool
{
    public class FileTimer
    {
        private ILogger _logger;
        private Stopwatch _stopwatch = Stopwatch.StartNew();

        public FileTimer(ILogger logger)
        {
            _logger = logger;
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _logger.Info("Time taken: " + TimeInMins + " mins");
        }

        private double TimeInMins
        {
            get { return Math.Round(_stopwatch.Elapsed.TotalMinutes, 1); }
        }
    }
}
