
using System;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
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

            // IMPORTANT - when composite area types are added here then they 
            // must also be added to the same class in FPM
            foreach (var areaTypeId in areaTypeIds)
            {
                switch (areaTypeId)
                {
                    case AreaTypeIds.CountyAndUnitaryAuthority:
                        Ids.Add(AreaTypeIds.County);
                        Ids.Add(AreaTypeIds.UnitaryAuthority);
                        break;

                    case AreaTypeIds.DistrictAndUnitaryAuthority:
                        Ids.Add(AreaTypeIds.District);
                        Ids.Add(AreaTypeIds.UnitaryAuthority);
                        break;

                    case AreaTypeIds.PheCentresFrom2013To2015:
                        Ids.Add(AreaTypeIds.PheCentreFrom2013);
                        Ids.Add(AreaTypeIds.PheCentreFrom2013To2015);
                        break;

                    case AreaTypeIds.PheCentresFrom2015:
                        Ids.Add(AreaTypeIds.PheCentreFrom2015);
                        Ids.Add(AreaTypeIds.PheCentreFrom2013To2015);
                        break;

                    case AreaTypeIds.MentalHealthTrustsIncludingCombinedAcuteTrusts:
                        Ids.Add(AreaTypeIds.MentalHealthTrust);
                        Ids.Add(AreaTypeIds.CombinedMentalHealthAndAcuteTrust);
                        break;

                    case AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts:
                        Ids.Add(AreaTypeIds.AcuteTrust);
                        Ids.Add(AreaTypeIds.CombinedMentalHealthAndAcuteTrust);
                        break;

                    default:
                        Ids.Add(areaTypeId);
                        break;
                }
            }
        }
    }
}
