using FingertipsUploadService.ProfileData.Entities.Core;

namespace FingertipsUploadService.ProfileData.Helpers
{
    public static class Mapper
    {
        public static CoreDataSet ToCoreDataSet(this UploadDataModel uploadDataModel)
        {
            var coreDataSet = new CoreDataSet
            {
                AgeId = uploadDataModel.AgeId,
                AreaCode = uploadDataModel.AreaCode,
                CategoryId = uploadDataModel.CategoryId,
                CategoryTypeId = uploadDataModel.CategoryTypeId,
                Count = uploadDataModel.Count,
                Denominator = uploadDataModel.Denominator,
                Denominator_2 = uploadDataModel.Denominator_2,
                IndicatorId = uploadDataModel.IndicatorId,
                LowerCi = uploadDataModel.LowerCi,
                Month = uploadDataModel.Month,
                Quarter = uploadDataModel.Quarter,
                SexId = uploadDataModel.SexId,
                Uid = uploadDataModel.Uid,
                UploadBatchId = uploadDataModel.UploadBatchId,
                UpperCi = uploadDataModel.UpperCi,
                Value = uploadDataModel.Value,
                ValueNoteId = uploadDataModel.ValueNoteId,
                Year = uploadDataModel.Year,
                YearRange = uploadDataModel.YearRange
            };

            return coreDataSet;
        }

        public static CoreDataSetArchive ToCoreDataSetArchive(this CoreDataSet coreDataSet)
        {
            //            var coreDataSetArchive = new CoreDataSetArchive();
            //            AutoMapper.Mapper.Map(coreDataSet, coreDataSetArchive);
            //            return coreDataSetArchive;
            var coreDataSetArchive = new CoreDataSetArchive
            {
                IndicatorId = coreDataSet.IndicatorId,
                Year = coreDataSet.Year,
                YearRange = coreDataSet.YearRange,
                Quarter = coreDataSet.Quarter,
                Month = coreDataSet.Month,
                AgeId = coreDataSet.AgeId,
                SexId = coreDataSet.SexId,
                AreaCode = coreDataSet.AreaCode,
                Count = coreDataSet.Count,
                Value = coreDataSet.Value,
                LowerCi = coreDataSet.LowerCi,
                UpperCi = coreDataSet.UpperCi,
                Denominator = coreDataSet.Denominator,
                Denominator_2 = coreDataSet.Denominator_2,
                ValueNoteId = coreDataSet.ValueNoteId,
                Uid = coreDataSet.Uid,
                ReplacedByUploadBatchId = coreDataSet.UploadBatchId
            };
            return coreDataSetArchive;

        }

    }
}
