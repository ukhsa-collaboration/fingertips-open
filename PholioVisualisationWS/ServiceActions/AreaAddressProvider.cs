using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class AreaAddressProvider
    {
        private IAreasReader _areasReader;

        public AreaAddressProvider(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public AreaAddress GetAreaAddress(string areaCode)
        {
            var area = AreaFactory.NewArea(_areasReader, areaCode);
            if (area is CategoryArea || area is NearestNeighbourArea)
            {
                return new AreaAddress(area);
            }

            AreaAddress areaAddress = _areasReader.GetAreaWithAddressFromCode(areaCode);
            areaAddress.CleanAddress();
            areaAddress.LatitudeLongitude = MapCoordinateConverter.GetLatitudeLongitude(areaAddress.EastingNorthing);
            return areaAddress;
        }

        public IList<AreaAddress> GetAreaAddressList(IList<string> areaCodes)
        {
            var areaAddressList = new List<AreaAddress>();

            foreach (var areaCode in areaCodes)
            {
                areaAddressList.Add(GetAreaAddress(areaCode));
            }

            return areaAddressList;
        }
    }
}