
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PholioVisualisation.RequestParameters
{
    public class TrendDataParameters : GroupDataWithGroupingsParameters
    {
        public TrendDataParameters(NameValueCollection parameters)
            : base(parameters)
        {
        }
    }
}
