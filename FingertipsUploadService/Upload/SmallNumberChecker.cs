using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using System.Data;

namespace FingertipsUploadService.Upload
{
    public class SmallNumberChecker
    {
        private readonly IDisclosureControlRepository _disclosureControlRepository;
        private readonly ProfilesReader _profilesReader;
        private readonly AreaTypeRepository _areaTypeRepository;

        public SmallNumberChecker(ProfilesReader profilesReader,
            IDisclosureControlRepository disclosureControlRepository,
            AreaTypeRepository areaTypeRepository)
        {
            _profilesReader = profilesReader;
            _disclosureControlRepository = disclosureControlRepository;
            _areaTypeRepository = areaTypeRepository;
        }

        public void Check(DataRow row, int rowNumber, BatchUpload batchUpload)
        {
            var indicatorDisclosureControlId = GetIndicatorDisclosureControlId(row);

            if (indicatorDisclosureControlId != DisclosureControlIds.NoCheckRequired)
            {
                var disclosure = _disclosureControlRepository.GetDisclosureControlById(indicatorDisclosureControlId);
                if (disclosure != null)
                {
                    if (ShouldWarnForSmallNumberByAreaType(row) == false) return;

                    var smallNumberFormula = disclosure.Formula;
                    var count = row.Field<double>(UploadColumnNames.Count);
                    var isSmallNumber = new SmallNumberFormulaHelper().IsSmallNumber(count, smallNumberFormula);
                    if (isSmallNumber)
                    {
                        batchUpload.SmallNumberWarnings.Add(new SmallNumberWarning(rowNumber + 1, count));
                    }
                }
            }
        }

        private int GetIndicatorDisclosureControlId(DataRow row)
        {
            var indicatorId = row.Field<double>(UploadColumnNames.IndicatorId);
            var indicatorDisclosureControlId = _profilesReader
                .GetIndicatorMetadata((int)indicatorId).DisclosureControlId;
            return indicatorDisclosureControlId;
        }

        private bool ShouldWarnForSmallNumberByAreaType(DataRow row)
        {
            var areaCode = row.Field<string>(UploadColumnNames.AreaCode);
            return _areaTypeRepository.ShouldWarnAboutSmallNumbersForArea(areaCode);
        }
    }
}
