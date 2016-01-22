using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.Search
{
    public class IndicatorSearch
    {
        private ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        public IEnumerable<IndicatorMetadataTextValue> SearchByText(string searchText)
        {
            return GetOrderedAndDistinct(
                _reader.SearchIndicatorMetadataTextValuesByText("%" + searchText + "%"));
        }

        public IEnumerable<IndicatorMetadataTextValue> SearchByIndicatorId(int indicatorId)
        {
            return GetOrderedAndDistinct(
                _reader.SearchIndicatorMetadataTextValuesByIndicatorId(indicatorId));
        }

        private IEnumerable<IndicatorMetadataTextValue> GetOrderedAndDistinct(
            IEnumerable<IndicatorMetadataTextValue> results)
        {
            return results.OrderBy(x => x.IndicatorId)
                .Distinct(new IndicatorMetadataTextValueComparer());
        }
    }
}