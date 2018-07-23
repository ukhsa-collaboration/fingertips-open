using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class FileLoggerTest
    {
        private const string Path = @"c:\fingertips\FileLoggerTest.txt";

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
