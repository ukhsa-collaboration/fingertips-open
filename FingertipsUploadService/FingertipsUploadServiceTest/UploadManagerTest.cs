using System.Collections.Generic;
using FingertipsUploadService;
using FingertipsUploadService.Entities.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FingertipsUploadServiceTest
{
    [TestClass]
    public class UploadManagerTest
    {
        [TestMethod]
        public void TestStartJob()
        {
            var uploadManager = new UploadManager();
            

            uploadManager.ProcessUploadJobs();
        }

    }


}
