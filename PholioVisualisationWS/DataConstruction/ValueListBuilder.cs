using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ValueListBuilder
    {
        private IEnumerable<ValueData> dataList;

        public ValueListBuilder(IEnumerable<ValueData> dataList)
        {
            this.dataList = dataList;
        }

        /// <summary>
        /// Gets the valid values only as an unorder list. Invalid values are ignored.
        /// </summary>
        public IList<double> ValidValues
        {
            get
            {
                if (dataList == null)
                {
                    return new List<double>();
                }

                return dataList
                    .Where(x => x.IsValueValid)
                    .Select(x => x.Value)
                    .ToList();
            }
        }
    }
}
