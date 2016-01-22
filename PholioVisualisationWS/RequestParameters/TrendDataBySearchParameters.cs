
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class TrendDataBySearchParameters : GroupDataAtDataPointBySearchParameters
    {
        public TrendDataBySearchParameters(NameValueCollection parameters)
            : base(parameters)
        {
        }
    }
}
