using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace FingertipsDataExtractionTool
{
    public interface IPracticePopulationFileGenerator
    {
        void Generate();
    }

    public class PracticePopulationFileGenerator : IPracticePopulationFileGenerator
    {
        private const string QuinaryPopulationKey = "qp";

        private ILogger _logger;
        private IExcelFileWriter _excelFileWriter;

        public PracticePopulationFileGenerator(ILogger logger, 
            IExcelFileWriter excelFileWriter)
        {
            _logger = logger;
            _excelFileWriter = excelFileWriter;
        }

        public void Generate()
        {
            _logger.Info("About to start generating population files for Practice Profiles");
            CreatePopulationFile();
            _logger.Info("Practice profiles generation completed.");
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
            var builder = new PracticeProfileDataBuilder()
            {
                AreaCode = AreaCodes.England,
                GroupIds = new List<int> { groupId },
                ParentAreaTypeId = AreaTypeIds.Ccg
            };

            var workBook = builder.BuildWorkbook();
            var fileInfo = new PracticeProfileFileInfo(profileKey,
                new List<int> { groupId }, AreaCodes.England);
            _excelFileWriter.Write(fileInfo, workBook);
        }
    }
}
