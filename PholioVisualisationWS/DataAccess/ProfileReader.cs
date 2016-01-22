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
        bool CanPdfBeGenerated(int profileId, int areaTypeId);
        IgnoredAreaCodes GetAreaCodesToIgnore(int profileId);
        IList<int> GetGroupIdsFromAllProfiles();
        IList<int> GetGroupIdsFromSpecificProfiles(IList<int> profileIds);
        Profile GetProfile(int profileId);
        IList<ProfileConfig> GetAllProfiles();
        IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds, int areaTypeId);
        ProfileConfig GetProfileConfig(int profileId);

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

        public bool CanPdfBeGenerated(int profileId, int areaTypeId)
        {
            return CurrentSession.CreateCriteria<ProfilePdf>()
                .Add(Restrictions.Eq("ProfileId", profileId))
                .Add(Restrictions.Eq("AreaTypeId", areaTypeId))
                .UniqueResult<ProfilePdf>() != null;
        }

        public virtual IgnoredAreaCodes GetAreaCodesToIgnore(int profileId)
        {
            var profileConfig = GetProfileConfig(profileId);
            return new IgnoredAreaCodesInitialiser(profileConfig).Initialised;
        }

        public virtual IList<int> GetGroupIdsFromAllProfiles()
        {
            var q =CurrentSession.CreateSQLQuery(
                    "SELECT DISTINCT g.GroupID FROM GroupingMetadata g JOIN UI_Profiles p ON g.ProfileID = p.id WHERE exclude_indicators_from_search <> :AreIndicatorsExcludedFromSearch");
             q.SetParameter("AreIndicatorsExcludedFromSearch", true);
            return q.List<int>();
        }

        public virtual IList<ProfilePerIndicator> GetProfilesForIndicators(List<int> indicatorIds, int areaTypeId)
        {
            var q = CurrentSession.GetNamedQuery("GetProfilesForIndicatorIds");
            q.SetParameterList("indicatorIds", indicatorIds);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetResultTransformer(Transformers.AliasToBean<ProfilePerIndicator>());
            return q.List<ProfilePerIndicator>();
        }

        public virtual IList<int> GetGroupIdsFromSpecificProfiles(IList<int> profileIds)
        {
            var groupIds = CurrentSession.CreateCriteria<GroupingMetadata>()
                .Add(Restrictions.In(PropertyProfileId, profileIds.ToArray()))
                .SetProjection(Projections.Property("Id"))
                .List<int>();
            return groupIds;
        }

        public virtual Profile GetProfile(int profileId)
        {
            var profileConfig = GetProfileConfig(profileId);
            return new ProfileInitialiser(profileConfig).InitialisedProfile;
        }

        public virtual IList<ProfileConfig> GetAllProfiles()
        {
            var profiles = CurrentSession.CreateCriteria<ProfileConfig>()
                .List<ProfileConfig>();
            return profiles;
            
        } 

        public virtual ProfileConfig GetProfileConfig(int profileId)
        {
            var profileConfig = CurrentSession.CreateCriteria<ProfileConfig>()
                                              .Add(Restrictions.Eq(PropertyProfileId, profileId))
                                              .UniqueResult<ProfileConfig>();
            return profileConfig;
        }
    }
}
