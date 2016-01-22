using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public static class AuthorDetails
    {
        public static string GetEmailAddress(int profileId)
        {
            if (profileId == ProfileIds.Phof)
            {
                return "phof.enquiries@phe.gov.uk";
            }

            return "profilefeedback@phe.gov.uk";
        }
    }
}
