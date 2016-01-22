using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.Search
{
    public class IndicatorNameComparer : IEqualityComparer<GroupingPlusName>
    {
        public bool Equals(GroupingPlusName x, GroupingPlusName y)
        {
            return x.IndicatorId == y.IndicatorId &&
                   x.AreaTypeId == y.AreaTypeId &&
                   x.AgeId == y.AgeId &&
                   x.SexId == y.SexId &&
                   x.YearRange == y.YearRange;
        }

        public int GetHashCode(GroupingPlusName obj)
        {
            return obj.IndicatorId.GetHashCode() ^
                   obj.AreaTypeId.GetHashCode() ^
                   obj.AgeId.GetHashCode() ^
                   obj.SexId.GetHashCode() ^
                   obj.YearRange.GetHashCode();
        }
    }
}