using System;
using System.Collections.Generic;
using FingertipsDataExtractionTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using SpreadsheetGear;

namespace PholioVisualisation.FingertipsDataExtractionToolTest
{
    [TestClass]
    public class PracticeProfilesExcelFileGeneratorTest
    {
        private Mock<IExcelFileWriter> _excelFileWriter;
        private Mock<ILogger> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger>(MockBehavior.Loose);

            _excelFileWriter = new Mock<IExcelFileWriter>(MockBehavior.Strict);
            _excelFileWriter.Setup(x => x.Write(It.IsAny<BaseExcelFileInfo>(), It.IsAny<IWorkbook>()))
                .Returns(new byte[] { });
        }

        [TestMethod]
        public void TestGeneratePopulationFileAndFileForAnotherDomain()
        {
            var generator = GetGenerator();

            generator.Generate();

            _excelFileWriter.VerifyAll();
            _logger.Verify(x => x.Error(It.IsAny<string>()), Times.Never);
        }

        private PracticePopulationFileGenerator GetGenerator()
        {
            var generator = new PracticePopulationFileGenerator(_logger.Object,
               _excelFileWriter.Object);
            return generator;
        }
    }
}
