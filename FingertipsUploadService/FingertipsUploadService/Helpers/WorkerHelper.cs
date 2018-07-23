using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.Upload;
using System.Data;
using UploadJob = FingertipsUploadService.ProfileData.Entities.Job.UploadJob;

namespace FingertipsUploadService.Helpers
{
    public class WorkerHelper
    {
        public static void UpdateNumberOfRowsInFile(UploadJob job, DataTable dataTable, UploadJobRepository repository)
        {
            var rowCount = 0;

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                var rowParser = new UploadRowParser(row);

                if (rowParser.DoesRowContainData == false)
                {
                    break;
                }

                rowCount++;
            }

            job.TotalRows = rowCount;
            repository.UpdateJob(job);
        }
    }
}