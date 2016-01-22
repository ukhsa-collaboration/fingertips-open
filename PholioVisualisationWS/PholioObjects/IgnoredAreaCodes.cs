using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class IgnoredAreaCodes
    {
        /// <summary>
        /// Area codes that should be ignored in all contexts for a particular profile.
        /// </summary>
        public IList<string> AreaCodesIgnoredEverywhere { get; set; }

        /// <summary>
        /// Area codes that should be ignored when calculating aggregate statistics. Also includes
        /// all the area codes of AreaCodesIgnoredEverywhere.
        /// </summary>
        public IList<string> AreaCodesIgnoredForSpineChart { get; set; }
    }
}
