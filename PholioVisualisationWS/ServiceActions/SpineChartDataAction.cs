using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PdfData;

namespace PholioVisualisation.ServiceActions
{
    public class SpineChartDataAction
    {
        public List<SpineChartTableData> GetResponse(int profileId, int childAreaTypeId, IList<string> areaCodes, string benchmarkAreaCode)
        {
            ParameterCheck.GreaterThanZero("Profile ID", profileId);
            ParameterCheck.GreaterThanZero("Child area type ID", childAreaTypeId);
            foreach (var areaCode in areaCodes)
            {
                ParameterCheck.ValidAreaCode(areaCode);
            }

            return new SpineChartTableDataBuilder().GetDomainDataForProfile(profileId, childAreaTypeId, areaCodes, new List<string>{benchmarkAreaCode});
        }

    }
}
