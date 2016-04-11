using System.Collections.Generic;
using System.Linq;
using Ckan.Model;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

namespace Ckan.DataTransformation
{
    /// <summary>
    /// Parameters required for uploading a group and packages to CKAN
    /// </summary>
    public class ProfileParameters
    {
        // Profile specific parameters
        public string EmailAddress = "PHOF.enquiries@phe.gov.uk";
        public string ProfileUrl = "http://www.phoutcomes.info";

        // Common for all profiles
        public string OrganisationTitle = "Public Health England";
        public string OrganisationId = OrganisationNames.PublicHealthEngland;

        // List of available licences api/action/license_list
        public string LicenceId = "uk-ogl";

        /// <summary>
        /// Empty constructor for testing
        /// </summary>
        public ProfileParameters(){}

        public ProfileParameters(IAreaTypeListProvider areaTypeListProvider,
            int profileId, string ckanGroupName)
        {
            ProfileId = profileId;
            CkanGroupName = ckanGroupName;

            var areaTypeIds = areaTypeListProvider.GetAllAreaTypeIdsUsedInProfile(profileId);
            AreaTypeIds = new AreaTypeIdSplitter(areaTypeIds).Ids.Distinct().ToList();

            CategoryTypeIds = areaTypeListProvider.GetCategoryTypeIdsUsedInProfile(profileId);
        }

        public string CkanGroupName { get; set; }
        public int ProfileId { get; private set; }
        public IList<int> AreaTypeIds { get; set; }
        public IList<int> CategoryTypeIds { get; set; }
    }
}