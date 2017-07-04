using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class AddressFileBuilder
    {
        private readonly CsvWriter _csvWriter = new CsvWriter();
        private readonly IAreasReader _areasReader;

        public AddressFileBuilder(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public byte[] GetAddressFile(int childAreaTypeId, string parentAreaCode)
        {
            AddHeader();
            IList<string> areaCodes = GetChildAreaCodes(childAreaTypeId, parentAreaCode);
            WriteAddresses(areaCodes);
            return _csvWriter.WriteAsBytes();
        }

        private IList<string> GetChildAreaCodes(int childAreaTypeId, string parentAreaCode)
        {
                Area area = _areasReader.GetAreaFromCode(parentAreaCode);

                return area.IsCountry
                    ? _areasReader.GetAreaCodesForAreaType(childAreaTypeId)
                    : _areasReader.GetChildAreaCodes(parentAreaCode, childAreaTypeId);
        }

        private void AddHeader()
        {
            _csvWriter.AddHeader("Code", "Name", "Address", "Postcode");
        }

        private void WriteAddresses(IList<string> areaCodes)
        {
            foreach (var areaCode in areaCodes)
            {
                AreaAddress address = _areasReader.GetAreaWithAddressFromCode(areaCode);
                if (address != null)
                {
                    _csvWriter.AddLine(
                        areaCode,
                        address.Name,
                        new AddressStringifier(address).AddressWithoutPostcode,
                        address.Postcode
                        );
                }
            }
        }

    }
}