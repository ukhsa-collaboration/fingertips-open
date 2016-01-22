using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class SupportingInformationForPdfsAction
    {
        public PdfSupportingInformation GetSupportingInformation(int profileId, string areaCode)
        {
            ParameterCheck.ValidAreaCode(areaCode);

            if (profileId == ProfileIds.HealthProfiles)
            {
                return new HealthProfilesSupportingInformationBuilder(areaCode).Build();
            }

            throw new FingertipsException("No supporting information available for requested profile");
        }

    }
}
