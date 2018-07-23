using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupingValidator
    {
        private readonly IList<Grouping> _groupings;

        public string AgesNotInLive { get; set; }

        public string AreaTypesNotInLive { get; set; }

        public GroupingValidator(IList<Grouping> groupings)
        {
            _groupings = groupings;
        }

        public void ValidateAges()
        {
            PholioReader reader = ReaderFactory.GetPholioReader();
            IList<int> ageIds = reader.GetAllAgeIds();

            foreach (Grouping grouping in _groupings)
            {
                if (!ageIds.Contains(grouping.AgeId))
                {
                    AgesNotInLive = string.Format("{0} {1}", AgesNotInLive, grouping.AgeId);
                }
            }
        }

        public void ValidateAreaTypes()
        {
            IList<int> areaTypeIds = ReaderFactory.GetAreasReader().GetAllAreaTypeIds();

            foreach (Grouping grouping in _groupings)
            {
                if (!areaTypeIds.Contains(grouping.AreaTypeId))
                {
                    AreaTypesNotInLive = string.Format("{0} {1}", AreaTypesNotInLive, grouping.AreaTypeId);
                }
            }
        }

        public string GetErrorMessage()
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrWhiteSpace(AgesNotInLive))
            {
                errorMessage = string.Format("The following ages does not exist on Live: {0}", AgesNotInLive);
            }

            if (!string.IsNullOrWhiteSpace(AreaTypesNotInLive))
            {
                errorMessage = string.Format("The following area type ids does not exist on Live: {0}", AreaTypesNotInLive);
            }

            return errorMessage;
        }
    }
}
