using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PdfData
{
    public class DomainNationalValues : DomainData
    {
        public List<GroupRootNationalValues> IndicatorData { get; set; }

        public DomainNationalValues()
        {
            IndicatorData = new List<GroupRootNationalValues>();
        }
    }
}
