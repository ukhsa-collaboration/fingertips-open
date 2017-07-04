using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class IgnoredAreasFilter
    {
        private IList<string> areaCodesIgnoredEverywhere;

        /// <summary>
        /// To enable mock creation.
        /// </summary>
        protected IgnoredAreasFilter() { }

        public IgnoredAreasFilter(IList<string> areaCodesIgnoredEverywhere)
        {
            this.areaCodesIgnoredEverywhere = areaCodesIgnoredEverywhere;
        }

        public virtual IEnumerable<IArea> RemoveAreasIgnoredEverywhere(IEnumerable<IArea> areasToFilter)
        {
            var areas = areasToFilter.Cast<IArea>().ToList();
            return new AreaFilter(areas).RemoveWithAreaCode(areaCodesIgnoredEverywhere);
        }

        public virtual IEnumerable<string> RemoveAreaCodesIgnoredEverywhere(IEnumerable<string> areaCodesToFilter)
        {
            return areaCodesToFilter.Where(x => areaCodesIgnoredEverywhere.Contains(x) == false);
        }

    }
}
