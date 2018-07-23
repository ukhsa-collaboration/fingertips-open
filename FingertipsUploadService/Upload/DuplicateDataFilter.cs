using FingertipsUploadService.ProfileData;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    public class DuplicateDataFilter
    {
        public IList<UploadDataModel> RemoveDuplicateData(IList<UploadDataModel> dataToBeUploaded)
        {
            var noDuplicateData =
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
            return noDuplicateData;
        }

        public List<UploadDataModel> GetDuplicatedData(IList<UploadDataModel> dataList)
        {

            var dup = dataList.GroupBy(
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
                .Where(z => z.Count() > 1)
                .SelectMany(z => z);

            return dup.ToList();
        }

    }
}
