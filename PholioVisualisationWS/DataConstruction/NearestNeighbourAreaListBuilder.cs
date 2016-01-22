using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Creates a list of areas which are the nearest neighbours.
    /// </summary>
    public class NearestNeighbourAreaListBuilder
    {
        public IList<IArea> Areas { get; set; }

        public NearestNeighbourAreaListBuilder(IAreasReader areasReader, NearestNeighbourArea nearestNeighbourArea)
        {
            var areaCodes = nearestNeighbourArea.NeighbourAreaCodes;

            IList<string> newAreaCodes = new List<string>();

            // add the selected area to the list of neighbours 
            // it should always be on zeroth index
            newAreaCodes.Add(nearestNeighbourArea.AreaCodeOfAreaWithNeighbours);

            foreach (var areaCode in areaCodes)
            {
                newAreaCodes.Add(areaCode);
            }

            var areas = areasReader.GetAreasFromCodes(newAreaCodes);
            Areas = areas.Cast<IArea>().ToList();
        }
    }
}