using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class NearbyAreasBuilder
    {
        public IList<NearByAreas> Build(string easting, string northing, int areaTypeId)
        {
            var areas = ReaderFactory.GetAreasReader().GetNearbyAreas(easting, northing, areaTypeId);
            foreach (var area in areas)
            {
                area.LatLng = MapCoordinateConverter.ConvertEastingNorthingToLatitudeLongitude(
                    area.Easting, area.Northing);

                area.DistanceValF = NumericFormatter.Format1DP(area.Distance);
            }
            return areas;
        }
    }
}
