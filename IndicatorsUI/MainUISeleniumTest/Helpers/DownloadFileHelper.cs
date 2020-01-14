using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public class DownloadFileHelper
    {
        public const string DownloadFolder= @"C:\fingertips\download";

        /// <summary>
        /// Large timeout to give time for files to be generated
        /// </summary>
        private const double TimeoutInSeconds = 120;
        private const double IntervalToCheckInSeconds = 0.5;

        public static void DeleteAllFilesInDownloadDir()
        {
            if (Directory.Exists(DownloadFolder))
            {
                var files = Directory.EnumerateFiles(DownloadFolder);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Checks that the file was downloaded and that it was not empty
        /// </summary>
        public static void CheckFileWasDownloaded(string fileName)
        {
            int i = 0;
            while (i < TimeoutInSeconds / IntervalToCheckInSeconds)
            {
                if (FileExistsInDownloadFolder(fileName))
                {
                    // File exists now check it is not empty
                    var path = GetFilePath(fileName);
                    Assert.AreNotEqual(0, new FileInfo(path).Length, "Downloaded file was empty");

                    // Success
                    return;
                }

                WaitFor.ThreadWaitInSeconds(IntervalToCheckInSeconds);
                i++;
            }

            // File not downloaded
            Assert.Fail("File could not be downloaded: " + fileName);
        }

        public static List<CsvRow> DownloadCsvFile(string fileName)
        {
            EnsureDownloadFolderExists();

            int i = 0;
            while (i < TimeoutInSeconds / IntervalToCheckInSeconds)
            {
                if (FileExistsInDownloadFolder(fileName))
                {
                    return ReadCsvFile(fileName);
                }

                WaitFor.ThreadWaitInSeconds(IntervalToCheckInSeconds);
                i++;
            }

            // File not downloaded
            return null;
        }

        private static bool FileExistsInDownloadFolder(string fileName)
        {
            var path = GetFilePath(fileName);

            // Does file exist?
            var exists = File.Exists(path);
            if (exists == false)
            {
                return false;
            }

            // Has file finished downloading?
            var lastAccessTime = File.GetLastAccessTime(path);
            var hasFinishedDownloading = DateTime.Now.Subtract(lastAccessTime).TotalSeconds > 3;
            return hasFinishedDownloading;
        }

        private static List<CsvRow> ReadCsvFile(string fileName)
        {
            var filePath = GetFilePath(fileName);

            var records = new List<CsvRow>();

            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(reader))
                {

                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new CsvRow
                        {
                            IndicatorId = csv.GetField<int>("Indicator ID"),
                            IndicatorName = csv.GetField<string>("Indicator Name"),
                            AreaCode = csv.GetField<string>("Area Code"),
                            AreaType = csv.GetField<string>("Area Type"),
                            Age = csv.GetField<string>("Age"),
                            Sex = csv.GetField<string>("Sex"),
                            TimePeriod = csv.GetField<string>("Time period")
                        };
                        records.Add(record);
                    }
                }
            }

            return records;
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(DownloadFolder, fileName);
        }

        private static void EnsureDownloadFolderExists()
        {
            if (Directory.Exists(DownloadFolder) == false)
            {
                Directory.CreateDirectory(DownloadFolder);
            }
        }
    }
}