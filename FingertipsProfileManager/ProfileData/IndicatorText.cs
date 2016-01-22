using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData
{
    public class IndicatorText
    {
        public IndicatorMetadataTextProperty IndicatorMetadataTextProperty { get; set; }
        public string ValueGeneric { get; set; }
        public string ValueSpecific { get; set; }

        public string ValueSearch { get; set; }
        public int IndicatorId { get; set; }

        public bool HasSpecificValue()
        {
                return ValueSpecific != null;
        }

        public bool HasGenericValue()
        {
            return ValueGeneric != null;
        }
    }
}
