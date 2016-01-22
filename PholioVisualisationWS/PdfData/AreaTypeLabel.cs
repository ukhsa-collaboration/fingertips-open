using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    /// <summary>
    /// Generates a label of the area type for local authorities. Used
    /// in the Health Profiles.
    /// </summary>
    public class AreaTypeLabel
    {
        public const string County = "County";
        public const string UnitaryAuthority = "Unitary Authority";
        public const string District = "District";

        public static string GetLabelFromAreaCode(string areaCode)
        {
            string subString = areaCode.Substring(1, 2);

            switch (subString)
            {
                case "07":
                    return District;
                case "10":
                    return County;
                case "08":
                case "06":
                case "09":
                    return UnitaryAuthority;
            }

            throw new FingertipsException("Area code does not conform to expected pattern" + areaCode);
        }
    }
}
