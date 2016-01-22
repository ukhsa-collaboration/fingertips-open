using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParameters
{
    internal static class ParameterHelper
    {
        internal static int GetNonSearchProfileId(int profileId, int templateProfileId)
        {
            return profileId == ProfileIds.Search ?
                templateProfileId :
                profileId;
        }

    }
}
