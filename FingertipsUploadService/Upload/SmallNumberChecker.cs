using System;
using System.Collections.Generic;
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

        private Dictionary<int, int> _indicatorIdToDisclosureId = new Dictionary<int, int>();

        public SmallNumberChecker(ProfilesReader profilesReader,
            IDisclosureControlRepository disclosureControlRepository,
            AreaTypeRepository areaTypeRepository)
        {
            _profilesReader = profilesReader;
            _disclosureControlRepository = disclosureControlRepository;
            _areaTypeRepository = areaTypeRepository;
        }

        public void Check(DataRow row, int rowNumber, UploadJobAnalysis uploadJobAnalysis)
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
                        uploadJobAnalysis.SmallNumberWarnings.Add(new SmallNumberWarning(rowNumber + 1, count));
                    }
                }
            }
        }

        private int GetIndicatorDisclosureControlId(DataRow row)
        {
            var indicatorId = (int)row.Field<double>(UploadColumnNames.IndicatorId);

            // Check if disclosure ID already retrieved
            if (_indicatorIdToDisclosureId.ContainsKey(indicatorId))
            {
                return _indicatorIdToDisclosureId[indicatorId];
            }

            var metadata = _profilesReader.GetIndicatorMetadata(indicatorId);
            var disclosureControlId = metadata.DisclosureControlId;
            _indicatorIdToDisclosureId.Add(indicatorId, disclosureControlId);

            return disclosureControlId;
        }

        private bool ShouldWarnForSmallNumberByAreaType(DataRow row)
        {
            var areaCode = row.Field<string>(UploadColumnNames.AreaCode);
            return _areaTypeRepository.ShouldWarnAboutSmallNumbersForArea(areaCode);
        }
    }
}
