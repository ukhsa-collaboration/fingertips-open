using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;

namespace FingertipsDataExtractionTool
{
    public interface IPracticeProfilesExcelFileGenerator
    {
        void Generate();
    }

    public class PracticeProfilesExcelFileGenerator : IPracticeProfilesExcelFileGenerator
    {
        private const string QuinaryPopulationKey = "qp";
        private const string PracticeProfileKey = "pp";

        private ILogger _logger;
        private IGroupDataReader _groupDataReader;
        private IExcelFileWriter _excelFileWriter;

        public PracticeProfilesExcelFileGenerator(ILogger logger, IGroupDataReader groupDataReader,
            IExcelFileWriter excelFileWriter)
        {
            _logger = logger;
            _groupDataReader = groupDataReader;
            _excelFileWriter = excelFileWriter;
        }

        public void Generate()
        {
            _logger.Info("About to start generating excel files for Practice Profiles");
            CreatePopulationFile();
            CreateDataFiles();
            _logger.Info("Practice profiles generation completed.");
        }

        private void CreateDataFiles()
        {
            var groupIds = _groupDataReader.GetGroupingIds(ProfileIds.PracticeProfiles);
            foreach (var groupId in groupIds)
            {
                try
                {
                    _logger.Info(GetPracticeProfileLogText(groupId));
                    var watch = new ExcelFileTimer(_logger);
                    WriteExcelFile(PracticeProfileKey, groupId);
                    watch.Stop();
                }
                catch (Exception ex)
                {
                    HandleException(ex, groupId);
                }
            }
        }

        private void CreatePopulationFile()
        {
            int groupId = GroupIds.PracticeProfiles_SupportingIndicators;
            try
            {
                WriteExcelFile(QuinaryPopulationKey, groupId);
            }
            catch (Exception ex)
            {
                HandleException(ex, groupId);
            }
        }

        private void HandleException(Exception ex, int groupId)
        {
            var message = "Practice Profiles Excel file not generated for: " + groupId;
            _logger.Error(message);
            NLogHelper.LogException(_logger, ex);
            ExceptionLog.LogException(new FingertipsException(message, ex), null);
        }

        private void WriteExcelFile(string profileKey, int groupId)
        {
            var builder = new PracticeProfileDataBuilder(UsePopulationData(profileKey))
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int> { groupId },
                AreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            var fileInfo = new PracticeProfileFileInfo(profileKey,
                new List<int> { groupId }, AreaCodes.England);
            _excelFileWriter.Write(fileInfo, workBook);
        }

        private static bool UsePopulationData(string profileKey)
        {
            // Other possible key is "pp"
            return profileKey.Equals(QuinaryPopulationKey, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetPracticeProfileLogText(int groupId)
        {
            var sb = new StringBuilder();
            sb.Append("Creating Practice Profile Excel file for GroupId: ")
                .Append(groupId);
            return sb.ToString();
        }
    }
}
