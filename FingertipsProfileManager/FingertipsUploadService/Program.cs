using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DIResolver;

namespace FingertipsUploadService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            IoC.Register();

#if(!DEBUG)
            // Normal Code execution path via windows services
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new FingertipsUploadService() 
            };
            ServiceBase.Run(ServicesToRun);
#else
            // Debug code execution path
            var service = new FingertipsUploadService();
            service.Start();
            Thread.Sleep(Timeout.Infinite);
#endif
        }
    }
}
