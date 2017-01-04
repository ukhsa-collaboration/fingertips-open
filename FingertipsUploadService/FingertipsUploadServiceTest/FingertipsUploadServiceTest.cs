using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceProcess;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class FingertipsUploadServiceTest
    {
        [TestMethod]
        public void TestCheckJobs()
        {
            var service = new FingertipsUploadService.FingertipsUploadService();
            service.CheckJobs();
            service.Dispose();
        }

        [TestMethod]
        public void TestLog()
        {
            var service = new FingertipsUploadService.FingertipsUploadService();
            service.InitLogger();
            service.Log("FingertipsUploadServiceTest: Testing logging");
        }
    }
}