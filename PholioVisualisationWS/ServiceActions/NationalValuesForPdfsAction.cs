using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class NationalValuesForPdfsAction
    {
        public IList<DomainNationalValues> GetResponse(int profileId, int childAreaTypeId, IList<string> benchmarkAreaCodes)
        {
            ParameterCheck.GreaterThanZero("Profile ID", profileId);
            ParameterCheck.GreaterThanZero("Child area type ID", childAreaTypeId);

            if (childAreaTypeId == AreaTypeIds.GpPractice)
            {
                throw new FingertipsException("GP Practices not an allowed area type");
            }

            return new DomainNationalValuesBuilder()
                .GetDomainDataForProfile(profileId, childAreaTypeId, benchmarkAreaCodes);
        }
    }
}
