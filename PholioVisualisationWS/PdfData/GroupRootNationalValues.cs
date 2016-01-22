using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class GroupRootNationalValues : IndicatorData
    {
        /// <summary>
        /// Key is area code.
        /// </summary>
        public IDictionary<string, CoreDataSet> AreaValues = new Dictionary<string, CoreDataSet>();
    }
}
