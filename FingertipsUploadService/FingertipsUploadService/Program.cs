
using NLog;
using System.Threading;

namespace FingertipsUploadService
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
#if(DEBUG)
            StartDebug();
#else
            StartRelease();
#endif
        }

        private static void StartDebug()
        {
            var service = new FingertipsUploadService();
            service.Start();
            LogMessage("Started in Debug mode");
            Thread.Sleep(Timeout.Infinite);
        }

        private static void StartRelease()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    new FingertipsUploadService()
                };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);

            LogMessage("Started in Release mode");

        }

        private static void LogMessage(string error)
        {
            var logger = LogManager.GetLogger("Program");
            logger.Info(error);
        }

    }
}