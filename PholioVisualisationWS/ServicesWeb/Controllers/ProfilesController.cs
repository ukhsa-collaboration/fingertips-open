using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;

namespace ServicesWeb.Controllers
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
                var profileConfigs = ReaderFactory.GetProfileReader().GetAllProfiles();
                IList<Profile> profiles = new List<Profile>();
                foreach (var profileConfig in profileConfigs)
                {
                    if (profileConfig.ProfileId != ProfileIds.Search)
                    {
                        profiles.Add(new ProfileInitialiser(profileConfig).InitialisedProfile);
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
                return ReaderFactory.GetProfileReader().GetProfile(profile_id);
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
        /// Whether or not PDF reports can be generated for a profile
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("are_pdfs")]
        public bool ArePdfs(int profile_id = 0, int area_type_id = 0)
        {
            try
            {
                return ReaderFactory.GetProfileReader().CanPdfBeGenerated(profile_id, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a content string associated with a specific profile
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="key">Content key unique within the profile</param>
        [HttpGet]
        [Route("content")]
        public string GetContent(int profile_id, string key)
        {
            try
            {
                var reader = ReaderFactory.GetContentReader();
                ContentItem data = reader.GetContentForProfile(profile_id, key);
                return data != null ? data.Content : string.Empty;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
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
        public Dictionary<int, List<ProfilePerIndicator>> GetProfilesPerIndicator(string indicator_ids, int area_type_id)
        {
            try
            {
                var response = new ProfilePerIndicatorBuilder(ApplicationConfiguration.IsEnvironmentLive)
                    .Build(new IntListStringParser(indicator_ids).IntList, area_type_id);
                return response;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
    }
}
