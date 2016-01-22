using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.DataConstruction
{
    public class IgnoredAreasFilterFactory
    {
        public static IgnoredAreasFilter New(int profileId)
        {
            var profileReader = ReaderFactory.GetProfileReader();
            var areaCodes = profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredEverywhere;
            return new IgnoredAreasFilter(areaCodes);
        }
    }
}
