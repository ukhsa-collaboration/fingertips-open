using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class StaticReportsControllerTest
    {
        private const string ProfileDirectoryName = "test";
        private const string TimePeriodDirectoryName = "1900";
        private const string FileName = "test.pdf";

        private string profileDirectoryPath = Path.Combine(ApplicationConfiguration.StaticReportsDirectory, ProfileDirectoryName);

        [TestInitialize]
        public void TestInitialize()
        {
            // Create profile folder
            Directory.CreateDirectory(profileDirectoryPath);
        }

        [TestMethod]
        public void TestGetStaticReportWithoutTimePeriod()
        {
            // Arrange: create file
            var filePath = Path.Combine(profileDirectoryPath, FileName);
            File.WriteAllText(filePath, string.Empty);

            // Act: download file
            using (var response = new StaticReportsController().GetStaticReport(ProfileDirectoryName, FileName))
            {

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode);
            }

            // Clean up
            File.Delete(filePath);
        }

        [TestMethod]
        public void TestGetStaticReportWithTimePeriod()
        {
            // Arrange: create file
            var dateFolderPath = Path.Combine(profileDirectoryPath, TimePeriodDirectoryName);
            Directory.CreateDirectory(dateFolderPath);
            var filePath = Path.Combine(dateFolderPath, FileName);
            File.WriteAllText(filePath, string.Empty);

            // Act: download file
            using (var response = new StaticReportsController().GetStaticReport(
                ProfileDirectoryName, FileName, TimePeriodDirectoryName))
            {

                // Assert
                Assert.IsTrue(response.IsSuccessStatusCode);
            }

            // Clean up
            File.Delete(filePath);
        }

        [TestMethod]
        public void TestIsStaticReportAvailable_Returns_True()
        {
            // Arrange: create file
            var filePath = Path.Combine(profileDirectoryPath, FileName);
            File.WriteAllText(filePath, string.Empty);

            // Assert file exists
            Assert.IsTrue(new StaticReportsController().IsStaticReportAvailable(ProfileDirectoryName, FileName));

            // Clean up
            File.Delete(filePath);
        }

        [TestMethod]
        public void TestIsStaticReportAvailable_Returns_False()
        {
            // Assert file does not exist
            Assert.IsFalse(new StaticReportsController().IsStaticReportAvailable(ProfileDirectoryName, FileName));
        }
    }
}
