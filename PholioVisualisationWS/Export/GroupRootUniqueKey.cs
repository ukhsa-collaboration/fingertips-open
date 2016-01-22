using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class GroupRootUniqueKey
    {
        public GroupRootUniqueKey(GroupRoot groupRoot)
        {
            var sb = new StringBuilder();
            sb.Append(groupRoot.IndicatorId);
            sb.Append("-");
            sb.Append(groupRoot.SexId);
            sb.Append("-");
            sb.Append(groupRoot.AgeId);
            Key = sb.ToString();
        }

        public string Key { get; set; }
    }
}