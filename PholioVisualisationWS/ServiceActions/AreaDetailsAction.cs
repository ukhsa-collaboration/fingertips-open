using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.ServiceActions
{
    public class AreaDetailsAction
    {

        public object GetResponse(int profileId, int groupId, int childAreaTypeId, string areaCode)
        {
            return new LongerLivesAreaDetailsBuilder().GetAreaDetails(profileId, groupId, childAreaTypeId, areaCode);
        }
    }
}
