using System;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IExceptionLoggerWrapper
    {
        void LogException(Exception ex, string url);
    }

    /// <summary>
    /// Wrapper for static ExceptionLogger.
    /// </summary>
    public class ExceptionLoggerWrapper : IExceptionLoggerWrapper
    {
        public void LogException(Exception ex, string url)
        {
            ExceptionLogger.LogException(ex, url);
        }
    }
}