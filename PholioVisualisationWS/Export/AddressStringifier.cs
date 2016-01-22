using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class AddressStringifier
    {
        private AreaAddress address;

        public AddressStringifier(AreaAddress areaAddress)
        {
            this.address = areaAddress;
        }

        public string AddressWithoutPostcode
        {
            get
            {
                string[] addressParts = new [] { 
                    address.Address1, 
                    address.Address2, 
                    address.Address3, 
                    address.Address4 };

                var validAddressParts = addressParts.Where(x => !string.IsNullOrWhiteSpace(x));

                return string.Join(", ", validAddressParts);
            }
        }
    }
}
