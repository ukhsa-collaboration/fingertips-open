
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public bool IsNational { get; set; }
        public List<int> GroupIds { get; set; }

        public Profile(IEnumerable<int> groupIds)
        {
            GroupIds = new List<int>(groupIds);
        }

        /// <summary>
        /// i.e. is not search results, or other kind of ad hoc profile.
        /// </summary>
        public bool IsDefinedProfile
        {
            get { return Id != ProfileIds.Search; }
        }


    }
}
