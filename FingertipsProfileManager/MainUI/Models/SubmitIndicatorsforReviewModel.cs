using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class SubmitIndicatorsForReviewModel : BaseDataModel
    {
        public int FromGroupId { get; set; }
        public int ToGroupId { get; set; }
        public int AreaTypeId { get; set; }
        public IList<GroupingPlusName> IndicatorsToReview { get; set; }
        public bool Resubmission { get; set; }
    }
}