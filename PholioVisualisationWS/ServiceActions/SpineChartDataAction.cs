using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PdfData;

namespace PholioVisualisation.ServiceActions
{
    public class SpineChartDataAction
    {
        public List<SpineChartTableData> GetResponse(SpineChartDataParameters parameters)
        {
            ParameterCheck.GreaterThanZero("Profile ID", parameters.ProfileId);
            ParameterCheck.GreaterThanZero("Child area type ID", parameters.ChildAreaTypeId);
            foreach (var areaCode in parameters.AreaCodes)
            {
                ParameterCheck.ValidAreaCode(areaCode);
            }

            return new SpineChartTableDataBuilder()
                .GetDomainDataForProfile(parameters);
        }

    }
}
