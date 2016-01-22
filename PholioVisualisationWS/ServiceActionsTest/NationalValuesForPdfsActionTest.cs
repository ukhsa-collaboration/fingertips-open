using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServiceActionsTest
{
    [TestClass]
    public class NationalValuesForPdfsActionTest
    {
        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestPracticesNotAllowed()
        {
            new NationalValuesForPdfsAction().GetResponse(ProfileIds.PracticeProfiles, AreaTypeIds.GpPractice,
                new List<string>());
        }
    }
}
