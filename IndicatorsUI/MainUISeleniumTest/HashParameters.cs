using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainUISeleniumTest
{
    public class HashParameters
    {
        private IList<string> parameters = new List<string>();

        public string HashParameterString
        {
            get { return "#" + string.Join("/", parameters); }
        }

        public void AddParentAreaCode(string areaCode)
        {
            parameters.Add("par");
            parameters.Add(areaCode);
        }

        public void AddAreaCode(string areaCode)
        {
            parameters.Add("are");
            parameters.Add(areaCode);
        }

        public void AddParentAreaTypeId(int parentAreaTypeId)
        {
            parameters.Add("pat");
            parameters.Add(parentAreaTypeId.ToString());
        }

        public void AddAreaTypeId(int areaTypeId)
        {
            parameters.Add("ati");
            parameters.Add(areaTypeId.ToString());
        }

        public void AddIndicatorId(int indicatorId)
        {
            parameters.Add("iid");
            parameters.Add(indicatorId.ToString());
        }

        public void AddSexId(int sexId)
        {
            parameters.Add("sex");
            parameters.Add(sexId.ToString());
        }

        public void AddAgeId(int ageId)
        {
            parameters.Add("age");
            parameters.Add(ageId.ToString());
        }

        public void AddTabId(int pageId)
        {
            parameters.Add("page");
            parameters.Add(pageId.ToString());
        }

        public void Add(string key, string value)
        {
            parameters.Add(key);
            parameters.Add(value);
        }

        public void Add(string key, int value)
        {
            parameters.Add(key);
            parameters.Add(value.ToString());
        }
    }
}
