using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class DomainIndicatorsSearchResult : BaseDataModel
    {
        public DomainIndicatorsSearchResult()
        {
            Indicators = new List<GroupingPlusName>();
        }

        public List<GroupingPlusName> Indicators { get; set; }
        public int GroupId { get; set; }
        public int SequenceId { get; set; }
        public string GroupName { get; set; }
        public string ProfileName { get; set; }
    }
}