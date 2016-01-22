using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public static class ParameterCheck
    {
        public static void GreaterThanZero(string parameterName, double val)
        {
            if (val <= 0)
            {
                throw new FingertipsException(parameterName + " must be greater than zero.");
            }
        }

        public static void ValidAreaCode(string areaCode)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
            {
                throw new FingertipsException("Invalid area code");
            }
        }


        
    }
}
