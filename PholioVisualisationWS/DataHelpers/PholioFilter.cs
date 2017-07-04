using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.DataSorting
{
    public abstract class PholioFilter
    {

        protected static IEnumerable<string> GetCodesInLowerCase(IEnumerable<string> areaCodesToRemove)
        {
            var lowerCodes = areaCodesToRemove.Select(x => x.ToLower());
            return lowerCodes;
        }

        protected static bool IsAreaCodeListOk(IEnumerable<string> areaCodesToRemove)
        {
            return areaCodesToRemove != null && areaCodesToRemove.Any();
        }
    }
}
