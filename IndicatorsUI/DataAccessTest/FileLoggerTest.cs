using System;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class FileLoggerTest
    {
        private static readonly string Path = ConfigurationManager.AppSettings["LoggerTest"];

        [TestMethod]
        public void Test()
        {
            File.Delete(Path);
            FileLogger logger = new FileLogger(Path);
            logger.WriteException(new Exception("TEST"));
            logger.WriteLine("test line");
            logger.WriteException(new Exception("TEST"));
            logger.WriteLine("test line");
        }
    }
}
