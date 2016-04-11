
using System.Threading;
using NLog;

namespace FingertipsUploadService
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Program");

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
#if(DEBUG)
            // Debug code execution path
            var service = new FingertipsUploadService();
            service.Start();
            Logger.Info("Started in Debug");
            Thread.Sleep(Timeout.Infinite);
#else
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    new FingertipsUploadService()
                };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            Logger.Info("Started in Normal");
#endif
        }

    }
}