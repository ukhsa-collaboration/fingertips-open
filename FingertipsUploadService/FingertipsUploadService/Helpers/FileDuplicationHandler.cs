using FingertipsUploadService.ProfileData;
using FingertipsUploadService.Upload;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.Helpers
{
    public class FileDuplicationHandler
    {
        public DataTable RemoveDuplicatesInBatch(DataTable batchData)
        {
            var dataToBeUploaded = new List<UploadDataModel>();

            for (int i = 0; i < batchData.Rows.Count; i++)
            {
                var row = batchData.Rows[i];
                var rowParser = new BatchRowParser(row);

                dataToBeUploaded.Add(rowParser.GetUploadDataModel());
            }

            var noDupData =
                dataToBeUploaded.GroupBy(
                    x =>
                        new
                        {
                            x.IndicatorId,
                            x.Year,
                            x.YearRange,
                            x.Quarter,
                            x.Month,
                            x.AgeId,
                            x.SexId,
                            x.AreaCode,
                            x.CategoryTypeId,
                            x.CategoryId
                        })
                    .Select(y => y.First())
                    .ToList();

            // Create new table without duplicats
            var table = new UploadDataSchema().CreateEmptyTable();

            foreach (var r in noDupData)
            {
                table.Rows.Add(r.IndicatorId, r.Year, r.YearRange, r.Quarter, r.Month, r.AgeId, r.SexId, r.AreaCode,
                    r.Count, r.Value, r.LowerCi, r.UpperCi, r.Denominator, r.Denominator_2, r.ValueNoteId,
                    r.CategoryTypeId, r.CategoryId);
            }
            return table;
        }
    }
}
