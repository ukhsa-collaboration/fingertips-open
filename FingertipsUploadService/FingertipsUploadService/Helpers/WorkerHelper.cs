using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.Upload;
using System.Data;

namespace FingertipsUploadService.Helpers
{
    public class WorkerHelper
    {
        public static void UpdateNumberOfRowsInFile(UploadJob job, DataTable dataTable, UploadJobRepository repository,
            bool isSimpleUpload)
        {
            var rowCount = 0;

            if (isSimpleUpload)
            {
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];
                    var rowParser = new UploadSimpleRowParser(row);

                    if (rowParser.DoesRowContainData == false)
                    {
                        break;
                    }

                    rowCount++;
                }
            }
            else
            {
                rowCount = dataTable.Rows.Count;
            }

            job.TotalRows = rowCount;
            repository.SaveJob(job);


        }


    }
}