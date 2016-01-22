using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData
{
    public class AreaTypeIdSplitter
    {
        public List<int> Ids { get; set; }

        public AreaTypeIdSplitter(int areaTypeId)
        {
            SetIds(new List<int> { areaTypeId });
        }

        public AreaTypeIdSplitter(IEnumerable<int> areaTypeIds)
        {
            SetIds(areaTypeIds);
        }

        private void SetIds(IEnumerable<int> areaTypeIds)
        {
            Ids = new List<int>();

            foreach (var areaTypeId in areaTypeIds)
            {
                switch (areaTypeId)
                {
                    case AreaTypeIds.CountyAndUnitaryAuthority:
                        Ids.Add(AreaTypeIds.County);
                        Ids.Add(AreaTypeIds.UnitaryAuthority);
                        break;

                    case AreaTypeIds.LocalAuthorityAndUnitaryAuthority:
                        Ids.Add(AreaTypeIds.LocalAuthority);
                        Ids.Add(AreaTypeIds.UnitaryAuthority);
                        break;

                    default:
                        Ids.Add(areaTypeId);
                        break;
                }
            }
        }
    }
}
