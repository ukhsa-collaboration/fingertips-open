using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData.Entities.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class UploadHelperTest
    {
        [TestMethod]
        public void Test_Not_Supported_Status()
        {
            Assert.AreEqual("Unknown", UploadHelper.GetTextFromStatusCodeForActiveJobs(UploadJobStatus.FailedValidation));
        }

        [TestMethod]
        public void Test_Supported_Status()
        {
            Assert.AreEqual("In progress", UploadHelper.GetTextFromStatusCodeForActiveJobs(UploadJobStatus.InProgress));
        }
    }
}
