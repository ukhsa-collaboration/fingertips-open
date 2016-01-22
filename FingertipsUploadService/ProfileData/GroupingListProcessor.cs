using FingertipsUploadService.ProfileData.Entities.Profile;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData
{
    public class GroupingListProcessor
    {
        public void RecalculateSequences(IList<Grouping> groupings)
        {
            int sequence = 1;
            Grouping previousGrouping = null;
            foreach (var grouping in groupings)
            {
                if (previousGrouping != null &&
                    grouping.CanHaveSameSequence(previousGrouping) == false)
                {
                    sequence++;
                }

                grouping.Sequence = sequence;
                previousGrouping = grouping;
            }
        }
    }
}