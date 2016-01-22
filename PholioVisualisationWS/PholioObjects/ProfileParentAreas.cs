using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class ProfileParentAreas
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int ParentAreaTypeId { get; set; }
        public string ParentAreaCodeString { get; set; }

        public List<string> ParentAreaCodes { get; set; }

        public void Init()
        {
            string[] bits = ParentAreaCodeString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ParentAreaCodes = bits.Select(x => x.Trim()).ToList();
        }
    }
}
