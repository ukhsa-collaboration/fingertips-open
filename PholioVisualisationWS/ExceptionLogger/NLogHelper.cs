using System;
using NLog;

namespace PholioVisualisation.ExceptionLogging
{
    public class NLogHelper
    {
        public static void LogException(ILogger logger, Exception ex)
        {
            logger.Error(ex.Message);
            logger.Error(ex.GetType().Name);
            logger.Error(ex.StackTrace);
        } 
    }
}