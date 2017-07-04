using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class StaticReportsControllerEndPointTest
    {
        [TestMethod]
        public void TestGetAreaSearchResults()
        {
            byte[] data = GetData("static-reports/exists?" +
                "profile_key=a"+ 
                "&file_name=b");
            TestHelper.AssertDataContainsString(data, "false");
        }

        public byte[] GetData(string path)
        {
            return DataControllerEndPointTest.GetData(path);
        }
    }
}
