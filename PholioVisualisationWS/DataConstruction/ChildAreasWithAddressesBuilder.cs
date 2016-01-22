using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreasWithAddressesBuilder
    {
        public IList<AreaAddress> Build(string parentAreaCode, int areaTypeId)
        {
            var areas = ReaderFactory.GetAreasReader().GetChildAreas(parentAreaCode, areaTypeId);

            var areaCodes = areas.Select(x => x.Code).ToList();
            var areaAddresses = ReaderFactory.GetAreasReader().GetAreaWithAddressFromCodes(areaCodes);

            foreach (var areaAddress in areaAddresses)
            {
                areaAddress.CleanAddress();
                areaAddress.LatitudeLongitude = MapCoordinateConverter.GetLatitudeLongitude(areaAddress.EastingNorthing);
            }

            return areaAddresses;
        }
    }
}
