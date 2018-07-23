using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData
{
    public class IndicatorMetadataProvider
    {
        private readonly IProfilesReader _reader;

        public IndicatorMetadataProvider(IProfilesReader profilesReader)
        {
            _reader = profilesReader;
        }

        public IList<IndicatorMetadataTextValue> GetAllIndicatorsForProfile(int profileId)
        {
            // Get the list of group ids from group meta data related to the profile id
            IList<int> groupIds = _reader.GetGroupingIds(profileId);

            // Get the list of indicator ids based on the group ids from the groupings
            IList<int> indicatorIds = _reader.GetGroupingIndicatorIds(groupIds);

            // Get the list of indicator meta data text value objects based on the indicator ids and profile id
            IList<IndicatorMetadataTextValue> indicatorMetadataTextValues =
                _reader.GetIndicatorMetadataTextValuesByIndicatorIdsAndProfileId(indicatorIds, profileId);

            // Throw an exception if there is no indicator meta data text values
            // for the indicator ids and profile id combination
            if (indicatorMetadataTextValues == null)
            {
                throw new FpmException("No indicators found for the profile id " + profileId);
            }

            // Update the indicator name for the records with null values
            UpdateIndicatorName(ref indicatorMetadataTextValues);

            // Return the distinct list of indicator metadata text values
            return GetDistinctIndicatorMetadataTextValues(indicatorMetadataTextValues);
        }

        public string GetProfileName(int profileId)
        {
            // Return the profile name
            return _reader.GetProfileDetailsByProfileId(profileId).Name;
        }

        private void UpdateIndicatorName(ref IList<IndicatorMetadataTextValue> indicatorMetadataTextValues)
        {
            // Loop through the indicator meta data text values and update the name value
            // if it is null, fetching from the other record for the same indicator id
            foreach (IndicatorMetadataTextValue indicatorMetadataTextValue in indicatorMetadataTextValues)
            {
                if (indicatorMetadataTextValue.Name == null)
                {
                    IndicatorMetadataTextValue tmpIndicatorMetadataTextValue =
                        indicatorMetadataTextValues.FirstOrDefault(i =>
                            i.IndicatorId == indicatorMetadataTextValue.IndicatorId && i.Name != null);

                    if (tmpIndicatorMetadataTextValue != null)
                    {
                        indicatorMetadataTextValue.Name = tmpIndicatorMetadataTextValue.Name;
                    }
                }
            }
        }

        private IList<IndicatorMetadataTextValue> GetDistinctIndicatorMetadataTextValues(IList<IndicatorMetadataTextValue> indicatorMetadataTextValues)
        {
            // Initialise indicator meta data text value list
            IList<IndicatorMetadataTextValue> tmpIndicatorMetadataTextValues = new List<IndicatorMetadataTextValue>();

            // Sort the indicator meta data text values by indicator id ascending and then by profile id descending
            IList<IndicatorMetadataTextValue> sortedIndicatorMetadataTextValues = indicatorMetadataTextValues
                .OrderBy(i => i.IndicatorId)
                .ThenByDescending(i => i.ProfileId).ToList();

            // Loop through the sorted list of indicator meta data text values
            foreach (IndicatorMetadataTextValue indicatorMetadataTextValue in sortedIndicatorMetadataTextValues)
            {
                // Set the duplicate found to false
                bool duplicateFound = false;

                // Loop through the initialised indicator meta data text value list
                foreach (IndicatorMetadataTextValue tmpIndicatorMetadataTextValue in tmpIndicatorMetadataTextValues)
                {
                    // If the indicator id matches in the two lists then set the duplicate found to true
                    if (indicatorMetadataTextValue.IndicatorId == tmpIndicatorMetadataTextValue.IndicatorId)
                    {
                        duplicateFound = true;
                    }
                }

                // If the duplicate found is false, then add the indicator meta data text value to the initialised list
                if (!duplicateFound)
                {
                    tmpIndicatorMetadataTextValues.Add(indicatorMetadataTextValue);
                }
            }

            // Return
            return tmpIndicatorMetadataTextValues;
        }
    }
}
