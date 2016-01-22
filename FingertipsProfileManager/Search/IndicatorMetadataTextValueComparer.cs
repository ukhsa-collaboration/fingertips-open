using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.Search
{
    public class IndicatorMetadataTextValueComparer : IEqualityComparer<IndicatorMetadataTextValue>
    {
        public bool Equals(IndicatorMetadataTextValue x, IndicatorMetadataTextValue y)
        {
            return x.IndicatorId == y.IndicatorId
                   && x.ProfileId == y.ProfileId
                   && x.Name == y.Name;
        }

        public int GetHashCode(IndicatorMetadataTextValue obj)
        {
            return obj.IndicatorId.GetHashCode() ^
             obj.Name.GetHashCode() ^
             obj.ProfileId.GetHashCode();
        }
    }
}