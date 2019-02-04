using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Parsers;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class ProfilesController : BaseController
    {
        /// <summary>
        /// Get all profiles
        /// </summary>
        [HttpGet]
        [Route("profiles")]
        public IList<Profile> GetProfiles()
        {
            try
            {
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                var profileConfigs = ReaderFactory.GetProfileReader().GetAllProfiles();
                IList<Profile> profiles = new List<Profile>();
                foreach (var profileConfig in profileConfigs)
                {
                    if (profileConfig.ProfileId != ProfileIds.Search)
                    {
                        var profile = new ProfileInitialiser(profileConfig).InitialisedProfile;
                        profiles.Add(profile);
                        profile.GroupMetadata = groupDataReader.GetGroupingMetadataList(profile.GroupIds);
                    }
                }
                return profiles;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a specific profile
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        [HttpGet]
        [Route("profile")]
        public Profile GetProfile(int profile_id)
        {
            try
            {
                var profile = ReaderFactory.GetProfileReader().GetProfile(profile_id);
                profile.GroupMetadata = ReaderFactory.GetGroupDataReader().GetGroupingMetadataList(profile.GroupIds);
                return profile;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of the area types for which PDF reports are available for a specific profile
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        [HttpGet]
        [Route("profile/area_types_with_pdfs")]
        public IList<AreaType> GetAreaTypesWithPdfsForProfile(int profile_id)
        {
            try
            {
                var areaTypeIds = ReaderFactory.GetProfileReader()
                    .GetProfilePdfs(profile_id)
                    .Select(x => x.AreaTypeId);

                return ReaderFactory.GetAreasReader().GetAreaTypes(areaTypeIds);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets name and sequence of specific profile groups
        /// </summary>
        /// <param name="group_ids">Comma-separated list of profile group IDs</param>
        [HttpGet]
        [Route("group_metadata")]
        public IList<GroupingMetadata> GetGroupingMetadatas(string group_ids)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.GroupIds, group_ids);

                var parameters = new GroupingTreeParameters(nameValues);
                return new JsonBuilderGroupingTree(parameters).GetGroupingMetadatas();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets profile group subheadings for a specific area type and profile group
        /// </summary>
        [HttpGet]
        [Route("group_subheadings")]
        public IList<GroupingSubheading> GetGroupingSubheadings(int area_type_id, int group_id)
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            return groupDataReader.GetGroupingSubheadings(area_type_id, group_id);
        }

        /// <summary>
        /// For each indicator lists the profiles that it is included in
        /// </summary>
        /// <remarks>A dictionary of indicator ID to a list of profiles
        /// </remarks>
        /// <param name="indicator_ids">List of indicator IDs</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("profiles_containing_indicators")]
        public Dictionary<int, List<ProfilePerIndicator>> GetProfilesPerIndicator(string indicator_ids, int area_type_id = AreaTypeIds.Undefined)
        {
            try
            {
                var response = new ProfilePerIndicatorBuilder(ApplicationConfiguration.Instance.IsEnvironmentLive)
                    .Build(new IntListStringParser(indicator_ids).IntList, area_type_id);
                return response;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        private static IList<Profile> GetProfilesFromIds(IList<int> profileIds)
        {
            // Remove system profiles
            profileIds = ProfileFilter.RemoveSystemProfileIds(profileIds);

            var profileConfigs = ReaderFactory.GetProfileReader().GetProfilesById(profileIds);
            IList<Profile> profiles = new List<Profile>();
            foreach (var profileConfig in profileConfigs)
            {
                profiles.Add(new ProfileInitialiser(profileConfig).InitialisedProfile);
            }
            return profiles;
        }
    }
}
