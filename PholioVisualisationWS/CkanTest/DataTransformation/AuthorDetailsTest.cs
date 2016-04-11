using System;
using System.Collections.Generic;
using System.Linq;
using Ckan.DataTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.CkanTest.DataTransformation
{
    [TestClass]
    public class AuthorDetailsTest
    {
        [TestMethod]
        public void TestGetEmailAddress()
        {
            // PHOF specific email
            Assert.AreEqual("phof.enquiries@phe.gov.uk",
                AuthorDetails.GetEmailAddress(ProfileIds.Phof));

            // Default email
            Assert.AreEqual("profilefeedback@phe.gov.uk",
                AuthorDetails.GetEmailAddress(ProfileIds.HealthProfiles));
        }
    }
}
