using System.Collections.Generic;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.Search
{
    public class DistinctGroupComparer : IEqualityComparer<Grouping>
    {
        public bool Equals(Grouping x, Grouping y)
        {
            return x.IndicatorId == y.IndicatorId &&
                   x.AreaTypeId == y.AreaTypeId &&
                   x.SexId == y.SexId &&
                   x.AgeId == y.AgeId &&
                   x.YearRange == y.YearRange &&
                   x.GroupId == y.GroupId;
        }

        public int GetHashCode(Grouping obj)
        {
            return obj.IndicatorId.GetHashCode() ^
                   obj.AreaTypeId.GetHashCode() ^
                   obj.SexId.GetHashCode() ^
                   obj.AgeId.GetHashCode() ^
                   obj.YearRange.GetHashCode() ^
                   obj.GroupId.GetHashCode();
        }
    }
}