using System;
using System.Collections.Generic;
using FingertipsDataExtractionTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace FingertipsDataExtractionToolTest
{
    [TestClass]
    public class PracticeProfilesExcelFileGeneratorTest
    {
        private Mock<IExcelFileWriter> _excelFileWriter;
        private Mock<ILogger> _logger;
        private Mock<IGroupDataReader> _groupDataReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger>(MockBehavior.Loose);

            _groupDataReader = new Mock<IGroupDataReader>(MockBehavior.Strict);
            _groupDataReader.Setup(x => x.GetGroupingIds(ProfileIds.PracticeProfiles))
                .Returns(new List<int> { GroupIds.PracticeProfiles_Cvd });

            _excelFileWriter = new Mock<IExcelFileWriter>(MockBehavior.Strict);
            _excelFileWriter.Setup(x => x.Write(It.IsAny<BaseExcelFileInfo>(), It.IsAny<IWorkbook>()))
                .Returns(new byte[] { });
        }

        [TestMethod]
        public void TestGeneratePopulationFileAndFileForAnotherDomain()
        {
            var generator = GetGenerator();

            generator.Generate();

            _groupDataReader.VerifyAll();
            _excelFileWriter.VerifyAll();
            _logger.Verify(x => x.Error(It.IsAny<string>()), Times.Never);
        }

        private PracticeProfilesExcelFileGenerator GetGenerator()
        {
            var generator = new PracticeProfilesExcelFileGenerator(_logger.Object,
                _groupDataReader.Object, _excelFileWriter.Object);
            return generator;
        }
    }
}
