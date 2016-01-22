using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public static class SexTextAppender
    {
        public static string GetIndicatorName(string indicatorName, int sexId, bool stateSex)
        {
            if (stateSex)
            {
                string sex;

                switch (sexId)
                {
                    case SexIds.Persons:
                        sex = "Persons";
                        break;
                    case SexIds.Female:
                        sex = "Female";
                        break;
                    case SexIds.Male:
                        sex = "Male";
                        break;
                    default:
                        sex = string.Empty;
                        break;
                }

                if (sex != string.Empty)
                {
                    sex = " (" + sex + ")";
                }

                return indicatorName + sex;
            }
            return indicatorName;
        }
    }
}
