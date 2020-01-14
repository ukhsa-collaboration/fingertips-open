using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public interface IProfileReader
    {
        IList<ProfilePdf> GetProfilePdfs(int profileId);
        IgnoredAreaCodes GetAreaCodesToIgnore(int profileId);
        IList<int> GetGroupIdsFromAllProfiles();
        IList<int> GetGroupIdsFromSpecificProfiles(IList<int> profileIds);
        Profile GetProfile(int profileId);
        IList<ProfileConfig> GetAllProfiles();
        IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds);
        IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds, int areaTypeId);
        ProfileConfig GetProfileConfig(int profileId);
        IList<int> GetAllProfileIds();
        IList<ProfileConfig> GetProfilesById(IList<int> profile_ids);
        IList<int> GetLongerLivesProfileIds();
        IList<int> GetGroupIdsForProfile(int profileId);

        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }

    public class ProfileReader : BaseReader, IProfileReader
    {
        private const string PropertyProfileId = "ProfileId";

        /// <summary>
        /// For mock object creation
        /// </summary>
        public ProfileReader() { }

        internal ProfileReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<ProfilePdf> GetProfilePdfs(int profileId)
        {
            return CurrentSession.CreateCriteria<ProfilePdf>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ProfileId", profileId))
                .List<ProfilePdf>();
        }

        public virtual IgnoredAreaCodes GetAreaCodesToIgnore(int profileId)
        {
            var profileConfig = GetProfileConfig(profileId);
            return new IgnoredAreaCodesInitialiser(profileConfig).Initialised;
        }

        public virtual IList<int> GetGroupIdsFromAllProfiles()
        {
            // Error thrown if SetCacheable(true);
            var q =CurrentSession.CreateSQLQuery(
                    "SELECT DISTINCT g.GroupID FROM GroupingMetadata g JOIN UI_Profiles p ON g.ProfileID = p.id WHERE exclude_indicators_from_search <> :AreIndicatorsExcludedFromSearch");
            q.SetParameter("AreIndicatorsExcludedFromSearch", true);
            return q.List<int>();
        }

        public virtual IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds)
        {
            var q = CurrentSession.GetNamedQuery("GetProfilesForIndicatorIds");
            q.SetParameterList("indicatorIds", indicatorIds);
            q.SetResultTransformer(Transformers.AliasToBean<ProfilePerIndicator>());
            return q.List<ProfilePerIndicator>();
        }

        public virtual IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds, int areaTypeId)
        {
            var q = CurrentSession.GetNamedQuery("GetProfilesForIndicatorIdsAndAreaTypeId");
            q.SetParameterList("indicatorIds", indicatorIds);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetResultTransformer(Transformers.AliasToBean<ProfilePerIndicator>());
            return q.List<ProfilePerIndicator>();
        }

        public virtual IList<int> GetGroupIdsFromSpecificProfiles(IList<int> profileIds)
        {
            var q = CurrentSession.CreateSQLQuery(
                "SELECT DISTINCT g.GroupID FROM GroupingMetadata g JOIN UI_Profiles p ON g.ProfileID = p.id WHERE g.ProfileID in (:profileIds) and exclude_indicators_from_search <> :AreIndicatorsExcludedFromSearch");
            q.SetParameterList("profileIds", profileIds);
            q.SetParameter("AreIndicatorsExcludedFromSearch", true);
            return q.List<int>();
        }

        public virtual IList<int> GetGroupIdsForProfile(int profileId)
        {
            return GetGroupIdsFromSpecificProfiles(new List<int> {profileId});
        }

        public virtual Profile GetProfile(int profileId)
        {
            var profileConfig = GetProfileConfig(profileId);
            return new ProfileInitialiser(profileConfig).InitialisedProfile;
        }

        public virtual IList<ProfileConfig> GetAllProfiles()
        {
            var profiles = CurrentSession.CreateCriteria<ProfileConfig>()
                .SetCacheable(true)
                .List<ProfileConfig>();
            return profiles;
        }

        public virtual IList<int> GetAllProfileIds()
        {
            var ids = CurrentSession.CreateCriteria<ProfileConfig>()
                .SetCacheable(true)
                .SetProjection(Projections.Property("ProfileId"))
                .List<int>();
            return ids;
        }

        public IList<int> GetLongerLivesProfileIds()
        {
            return CurrentSession.CreateCriteria<LongerLivesProfile>()
                .SetCacheable(true)
                .SetProjection(Projections.Property("ProfileId"))
                .List<int>();
        }

        public virtual ProfileConfig GetProfileConfig(int profileId)
        {
            var profileConfig = CurrentSession.CreateCriteria<ProfileConfig>()
                .SetCacheable(true)
                .Add(Restrictions.Eq(PropertyProfileId, profileId))
                .UniqueResult<ProfileConfig>();
            return profileConfig;
        }

        public virtual IList<ProfileConfig> GetProfilesById(IList<int> profile_ids)
        {
            var profiles = CurrentSession.CreateCriteria<ProfileConfig>()
                .Add(Restrictions.In(PropertyProfileId, profile_ids.ToArray()))
                .List<ProfileConfig>();
            return profiles;
        }
    }
}
