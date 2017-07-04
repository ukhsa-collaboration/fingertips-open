using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class AddressStringifier
    {
        private AreaAddress _address;

        public AddressStringifier(AreaAddress areaAddress)
        {
            _address = areaAddress;
        }

        public string AddressWithoutPostcode
        {
            get
            {
                string[] addressParts = new [] { 
                    _address.Address1, 
                    _address.Address2, 
                    _address.Address3, 
                    _address.Address4 };

                var validAddressParts = addressParts.Where(x => !string.IsNullOrWhiteSpace(x));

                return string.Join(", ", validAddressParts);
            }
        }
    }
}
