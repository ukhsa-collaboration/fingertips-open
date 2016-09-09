using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace ConsoleApplication
{
    /// <summary>
    /// Writes the Health Profiles key messages for all areas to a file.
    /// </summary>
    public class WriteKeyMessagesToFile : IConsoleTask
    {
        private const string ExportPath = @"c:\temp\messagedata.txt";

        private IList<string> countyAreas;
        private IList<string> localAuthorityAreas;
        private IList<string> unitaryAuthorityAreas;

        private StreamWriter writer;

        public void Do()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("!!!START!!!");

            OpenFile();
            GetAreaCodes();
            WriteHeaders();
            WriteMessageToFile(localAuthorityAreas);
            WriteMessageToFile(countyAreas);
            WriteMessageToFile(unitaryAuthorityAreas);
            writer.Close();

            Console.WriteLine("---DONE---");
            Console.WriteLine(Math.Round((DateTime.Now - start).TotalSeconds, 1) + " seconds");
            Console.ReadLine();
        }

        private void OpenFile()
        {
            if (File.Exists(ExportPath))
            {
                File.Delete(ExportPath);
            }
            writer = File.CreateText(ExportPath);
        }

        private void GetAreaCodes()
        {
            IAreasReader areaReader = ReaderFactory.GetAreasReader();
            localAuthorityAreas = areaReader.GetAreaCodesForAreaType(AreaTypeIds.District);
            countyAreas = areaReader
                .GetAreaCodesForAreaType(AreaTypeIds.County)
                .Where(x => x != AreaCodes.CountyUa_Bedfordshire)
                .ToList();
            unitaryAuthorityAreas = areaReader.GetAreaCodesForAreaType(AreaTypeIds.UnitaryAuthority);
        }

        private void WriteMessageToFile(IList<string> areaCodes)
        {
            foreach (string areaCode in areaCodes)
            {
                if (areaCode == AreaCodes.CountyUa_IslesOfScilly || areaCode == AreaCodes.CountyUa_CityOfLondon)
                {
                    // do nothing
                }
                else
                {
                    HealthProfilesSupportingInformation data =
                        new HealthProfilesSupportingInformationBuilder(areaCode).Build();

                    var sb = new StringBuilder();
                    sb.Append(areaCode);
                    sb.Append("\t");
                    sb.Append(data.HealthProfilesData.Population);
                    sb.Append("\t");
                    sb.Append(data.HealthProfilesContent.AreaType);

                    IList<string> messages = data.HealthProfilesContent.KeyMessages;
                    foreach (string message in messages)
                    {
                        sb.Append("\t");
                        sb.Append(message);
                    }

                    WriteLineToFile(sb.ToString());
                }
            }
        }

        private void WriteHeaders()
        {
            var headers = new[]
            {"Area code", "Population", "Area type", "Message 1", "Message 2", "Message 3", "Message 4", "Message 5"};
            var sb = new StringBuilder();
            foreach (string header in headers)
            {
                sb.Append(header);
                sb.Append("\t");
            }
            WriteLineToFile(sb.ToString());
        }

        private void WriteLineToFile(string text)
        {
            try
            {
                text = text
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("’", "'");
                writer.WriteLine(text);
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
        }
    }
}