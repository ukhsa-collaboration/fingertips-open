using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    /// <summary>
    /// Note: for counties the composite area type is "Counties & UAs", otherwise
    /// it is "Districts & UAs"
    /// </summary>
    public class HealthProfilesAreaTypeHelper
    {
        public static int GetCompositeAreaTypeId(Area area)
        {
            int areaTypeId = area.IsCounty
                ? AreaTypeIds.CountyAndUnitaryAuthority
                : AreaTypeIds.DistrictAndUnitaryAuthority;
            return areaTypeId;
        }

        public static string GetAreaTypeName(int areaTypeId)
        {
            return areaTypeId == AreaTypeIds.County
                ? "counties/unitary authorities"
                : "districts/unitary authorities";
        }
    }
}