using FingertipsUploadService.ProfileData;
using FingertipsUploadService.Upload;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.Helpers
{
    public class FileDuplicationHandler
    {
        public DataTable RemoveDuplicatesInSimple(DataTable pholioData)
        {
            var simpleUpload = new SimpleUpload();
            var dataToBeUploaded = new List<UploadDataModel>();

            for (var i = 0; i < pholioData.Rows.Count; i++)
            {
                var row = pholioData.Rows[i];
                var rowParser = new UploadSimpleRowParser(row);

                if (rowParser.DoesRowContainData == false)
                {
                    //There isn't an area code or value so assume the end of the data 
                    break;
                }

                var uploadDataModel = rowParser.GetUploadDataModelWithUnparsedValuesSetToDefaults(simpleUpload);
                dataToBeUploaded.Add(uploadDataModel);
            }

            // Remove the duplicate
            var noDupData =
                        dataToBeUploaded.GroupBy(
                            x =>
                                new
                                {
                                    x.AreaCode
                                })
                            .Select(y => y.First())
                            .ToList();

            // Create new table without duplicats            
            var table = new DataTable();
            table.Columns.Add("AreaCode", typeof(string));
            table.Columns.Add("Count", typeof(double));
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("LowerCI", typeof(double));
            table.Columns.Add("UpperCI", typeof(double));
            table.Columns.Add("Denominator", typeof(double));
            table.Columns.Add("ValueNoteId", typeof(double));

            foreach (var data in noDupData)
            {
                table.Rows.Add(data.AreaCode, data.Count, data.Value, data.LowerCi, data.UpperCi, data.Denominator,
                    data.ValueNoteId);
            }
            return table;
        }


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
            var table = new DataTable();
            table.Columns.Add("IndicatorID", typeof(double));
            table.Columns.Add("Year", typeof(double));
            table.Columns.Add("YearRange", typeof(double));
            table.Columns.Add("Quarter", typeof(double));
            table.Columns.Add("Month", typeof(double));
            table.Columns.Add("AgeID", typeof(double));
            table.Columns.Add("SexID", typeof(double));
            table.Columns.Add("AreaCode", typeof(string));
            table.Columns.Add("Count", typeof(double));
            table.Columns.Add("Value", typeof(double));
            table.Columns.Add("LowerCI", typeof(double));
            table.Columns.Add("UpperCI", typeof(double));
            table.Columns.Add("Denominator", typeof(double));
            table.Columns.Add("Denominator_2", typeof(double));
            table.Columns.Add("ValueNoteId", typeof(double));
            table.Columns.Add("CategoryTypeId", typeof(double));
            table.Columns.Add("CategoryId", typeof(double));

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
