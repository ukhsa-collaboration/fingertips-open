using System.Collections.Generic;
using System.Data;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;

namespace FingertipsUploadService.Upload
{
    public interface IDataTableUploader
    {
        void UploadData(UploadJob job, UploadJobAnalysis jobAnalysis);
        void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job);
    }

    public class DataUploader : IDataTableUploader
    {
        private readonly CoreDataRepository _coreDataRepository;
        private readonly Logger _logger;

        public DataUploader(CoreDataRepository coreDataRepository, Logger logger)
        {
            _coreDataRepository = coreDataRepository;
            _logger = logger;
        }

        public void UploadData(UploadJob job, UploadJobAnalysis jobAnalysis)
        {
            var dataList = jobAnalysis.DataToUpload;
            int totalRows = dataList.Count;
            var indicatorIds = new HashSet<int>();

            // Upload core data
            for (int i = 0; i < dataList.Count; i++)
            {
                var coreData = dataList[i].ToCoreDataSet();
                _coreDataRepository.InsertCoreData(coreData, job.Guid);
                indicatorIds.Add(coreData.IndicatorId);

                // Log periodically that rows have been uploaded
                if (i % 1000 == 0)
                {
                    LogRowsUploaded(i + 1);
                }
            }

            LogRowsUploaded(totalRows);

            // Delete any precalculated core data as it will need to be calculated again
            foreach (var indicatorId in indicatorIds)
            {
                _coreDataRepository.DeletePrecalculatedCoreData(indicatorId);
            }
        }

        public void ArchiveDuplicates(IEnumerable<DuplicateRowInDatabaseError> duplicateRows, UploadJob job)
        {
            _coreDataRepository.InsertCoreDataArchive(duplicateRows, job.Guid);
        }

        private void LogRowsUploaded(int rowCount)
        {
            if (_logger != null)
            {
                _logger.Info(rowCount + " rows uploaded");
            }
        }
    }
}