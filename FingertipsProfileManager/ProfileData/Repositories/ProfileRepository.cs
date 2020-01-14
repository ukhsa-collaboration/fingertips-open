using Fpm.ProfileData.Entities.Logging;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Helpers;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.ProfileData.Repositories
{
    public class ProfileRepository : RepositoryBase, IProfileRepository
    {
        public ProfileRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public ProfileRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        /// <summary>
        /// Flush the session. May be required for testing to ensure changes are picked up.
        /// </summary>
        public void RefreshObject(object o)
        {
            CurrentSession.Flush();
            CurrentSession.Refresh(o);
            CurrentSession.Clear();
        }

        public IList<ProfileDetails> GetProfiles()
        {
            var q = CurrentSession.CreateQuery("from ProfileDetails p where p.Id != " + ProfileIds.Search);
            return q.List<ProfileDetails>();
        }


        public int CreateProfile(ProfileDetails profileDetails)
        {
            var newProfileId = ProfileIds.Undefined;

            try
            {
                transaction = CurrentSession.BeginTransaction();

                newProfileId = (int)CurrentSession.Save(profileDetails);

                profileDetails.UserPermissions.ToList()
                        .ForEach(u =>
                        {
                            u.ProfileId = newProfileId;
                            CurrentSession.Save(u);
                        });

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return newProfileId;
        }

        public void UpdateProfile(ProfileDetails profileDetails)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Delete("from UserGroupPermissions where profileId = " + profileDetails.Id);

                CurrentSession.Update(profileDetails);

                if (profileDetails.UserPermissions != null)
                    profileDetails.UserPermissions.ForEach(u =>
                    {
                        u.ProfileId = profileDetails.Id;
                        CurrentSession.Save(u);
                    });

                CurrentSession.GetNamedQuery("Delete_ProfilePDFs")
                    .SetParameter("ProfileId", profileDetails.Id)
                    .ExecuteUpdate();

                if (profileDetails.PdfAreaTypes != null)
                {
                    profileDetails.PdfAreaTypes.ForEach(a => CurrentSession.GetNamedQuery("Insert_ProfilePDFs")
                        .SetParameter("ProfileId", profileDetails.Id)
                        .SetParameter("AreaTypeId", a.Id)
                        .ExecuteUpdate());
                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void UpdateProfileDetail(ProfileDetails profileDetails)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Update(profileDetails);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void CreateIndicator(IEnumerable<IndicatorMetadataTextProperty> allPropertiesToAdd, int indicatorId)
        {
            var indicatorMetaDataTextValue = new IndicatorMetadataTextValue();

            try
            {
                transaction = CurrentSession.BeginTransaction();

                allPropertiesToAdd.ForEach(p => indicatorMetaDataTextValue.SetPropertyValue(p.ColumnName, p.Text));

                indicatorMetaDataTextValue.IndicatorId = indicatorId;

                CurrentSession.Save(indicatorMetaDataTextValue);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void CreateNewOverriddenIndicator(IndicatorMetadataTextProperty property, string text, int indicatorId,
            int? profileId)
        {
            var indicatorMetaDataTextValue = SetIndicatorMetaDataTextValue(property, text, indicatorId, profileId);

            CurrentSession.Save(indicatorMetaDataTextValue);
        }

        private static IndicatorMetadataTextValue SetIndicatorMetaDataTextValue(IndicatorMetadataTextProperty property, string text, int indicatorId, int? profileId)
        {
            var indicatorMetaDataTextValue = new IndicatorMetadataTextValue()
            {
                IndicatorId = indicatorId,
                ProfileId = profileId,
            };

            indicatorMetaDataTextValue.SetPropertyValue(property.ColumnName, text);
            indicatorMetaDataTextValue = SetIndicatorMetaDataTextValueInternalVariables(indicatorMetaDataTextValue);

            return indicatorMetaDataTextValue;
        }

        private static IndicatorMetadataTextValue SetIndicatorMetaDataTextValueInternalVariables(IndicatorMetadataTextValue indicatorMetaDataTextValue)
        {
            var indicatorMetadataTextPropertyInternalMetadataList = IndicatorMetadataTextProperty.GetIndicatorMetadataTextPropertyInternalMetadata();
            foreach (var property in indicatorMetadataTextPropertyInternalMetadataList)
            {
                indicatorMetaDataTextValue.SetPropertyValue(property.ColumnName, property.Text);
            }
            
            return indicatorMetaDataTextValue;
        }

        public bool DoesOverriddenIndicatorMetadataRecordAlreadyExist(int indicatorId, int? profileId)
        {
            var result = CurrentSession.CreateQuery(string.Format(
                "from IndicatorMetadataTextValue  where IndicatorId = {0} and ProfileId = {1}", indicatorId, profileId))
                    .List<IndicatorMetadataTextValue>();

            return result.Count > 0;
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId)
        {
            var q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            return q.UniqueResult<IndicatorMetadata>();
        }

        public int GetNextIndicatorId()
        {
            return
                CurrentSession.CreateCriteria(typeof(IndicatorMetadataTextValue))
                    .SetProjection(Projections.Max("IndicatorId"))
                    .UniqueResult<int>() + 1;
        }

        public int CreateGrouping(Grouping grouping)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                var newGroupId = (int)CurrentSession.Save(grouping);
                transaction.Commit();
                return newGroupId;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return -1;
        }

        public int CreateProfileCollection(ProfileCollection profileCollection, string assignedProfiles)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                var profileCollectionId = (int)CurrentSession.Save(profileCollection);

                if (!string.IsNullOrEmpty(assignedProfiles))
                {
                    assignedProfiles.Split(',')
                        .ForEach(profileId =>
                            CurrentSession.Save(new ProfileCollectionItem()
                            {
                                ProfileCollectionId = profileCollectionId,
                                ProfileId = Convert.ToInt32(profileId)
                            })
                        );
                }

                transaction.Commit();
                return profileCollectionId;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return -1;
        }

        public bool UpdateProfileCollection(int profileCollectionId, string assignedProfilesToUpdate, string collectionNameToUpdate, 
            string collectionSkinTitleToUpdate)
        {

            try
            {
                transaction = CurrentSession.BeginTransaction();
                var profileCollection = new ProfileCollection()
                {
                    Id = profileCollectionId,
                    CollectionName = collectionNameToUpdate,
                    CollectionSkinTitle = collectionSkinTitleToUpdate
                };

                CurrentSession.Delete("from ProfileCollectionItem where ProfileCollectionId = " + profileCollectionId);

                CurrentSession.Update(profileCollection);

                if (!string.IsNullOrEmpty(assignedProfilesToUpdate))
                {
                    foreach (var assignedProfile in assignedProfilesToUpdate.Split(','))
                    {
                        var bits = assignedProfile.Split('~');

                        var assignedProfileId = bits[0];
                        var showDomain = bits[1] == "true";

                        CurrentSession.Save(new ProfileCollectionItem()
                        {
                            ProfileCollectionId = profileCollectionId,
                            ProfileId = Convert.ToInt32(assignedProfileId),
                            DisplayDomains = showDomain
                        });

                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return false;
        }

        public bool ChangeOwner(int indicatorId, int newOwnerProfileId,
            IList<IndicatorText> newOwnerMetadataTextValues,
            IList<IndicatorText> currentOwnerMetadataTextValues)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                var metadata = GetIndicatorMetadata(indicatorId);

                if (newOwnerMetadataTextValues.Any())
                {
                    UpdateTextMetadata(indicatorId, newOwnerProfileId, metadata.OwnerProfileId,
                        newOwnerMetadataTextValues, currentOwnerMetadataTextValues);
                }

                // Update owner profile ID
                metadata.OwnerProfileId = newOwnerProfileId;
                CurrentSession.SaveOrUpdate(metadata);

                transaction.Commit();
                return true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return false;
        }

        private void UpdateTextMetadata(int indicatorId, int newOwnerProfileId, int oldOwnerProfileId,
            IList<IndicatorText> newOwnerMetadataTextValues,
            IList<IndicatorText> oldOwnerMetadataTextValues)
        {

            // Change overridden metadata row so it applies to old owner
            var queryString = string.Format(
                @"update IndicatorMetadataTextValue set ProfileId = {0} where IndicatorId = {1} and ProfileId = {2}",
                oldOwnerProfileId, indicatorId, newOwnerProfileId);
            CurrentSession.CreateQuery(queryString).ExecuteUpdate();

            for (var i = 0; i < newOwnerMetadataTextValues.Count; i++)
            {
                var oldOwnerTextValue = oldOwnerMetadataTextValues[i];
                var newOwnerTextValue = newOwnerMetadataTextValues[i];

                // Switch around properties where both are defined
                if (oldOwnerTextValue.HasGenericValue() && newOwnerTextValue.HasGenericValue())
                {
                    var metadataTextProperty = oldOwnerTextValue.IndicatorMetadataTextProperty;
                    UpdateProperty(metadataTextProperty, newOwnerTextValue.ValueGeneric, indicatorId, null);
                    UpdateProperty(metadataTextProperty, oldOwnerTextValue.ValueGeneric, indicatorId, oldOwnerProfileId);
                }
            }
        }

        public void UpdateProperty(IndicatorMetadataTextProperty property, string text, int indicatorId, int? profileId)
        {
            var profileMatch = profileId.HasValue ? "=" + profileId.Value : "IS NULL";

            var hqlQueryString = string.Format(
                @"update IndicatorMetadataTextValue set {0} = :text where IndicatorID = {1} AND ProfileId {2}",
                     property.ColumnName, indicatorId, profileMatch);

            var updateSession = CurrentSession.CreateQuery(hqlQueryString);

            updateSession.SetParameter("text", text);

            updateSession.ExecuteUpdate();
        }

        public bool CreateGroupingAndMetadata(IndicatorMetadata indicatorMetadata,
            IndicatorMetadataTextValue indicatorMetadataTextValue, Grouping grouping, string user)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(indicatorMetadataTextValue);

                CurrentSession.Save(indicatorMetadata);

                InsertGrouping(grouping);

                LogAuditChange(string.Format("New Indicator {0} Successfully Created", indicatorMetadata.IndicatorId),
                    indicatorMetadata.IndicatorId, grouping.GroupId, user, DateTime.Now, "Create");

                transaction.Commit();
                return true;

            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return false;
        }

        public bool UpdateGroupingAndMetadata(IndicatorMetadata indicatorMetadata,
            IndicatorMetadataTextValue indicatorMetadataTextValue, Grouping grouping, int oldAreaTypeId, int oldSexId,
            int oldAgeId, string user)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Update(indicatorMetadataTextValue);

                CurrentSession.Update(indicatorMetadata);

                UpdateGrouping(grouping, oldAreaTypeId, oldSexId, oldAgeId);

                LogAuditChange(string.Format("Indicator {0} Successfully Updated", indicatorMetadata.IndicatorId),
                    indicatorMetadata.IndicatorId, grouping.GroupId, user, DateTime.Now, "Create");

                transaction.Commit();
                return true;

            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return false;
        }

        private void InsertGrouping(Grouping grouping)
        {
            if (grouping.ComparatorId == ComparatorIds.Subnational && grouping.AreaTypeId == AreaTypeIds.GoRegion)
            {
                return;
            }

            foreach (var comparatorId in GetComparatorIds())
            {
                CurrentSession.Save(new Grouping()
                {
                    GroupId = grouping.GroupId,
                    SexId = grouping.SexId,
                    AgeId = grouping.AgeId,
                    AreaTypeId = grouping.AreaTypeId,
                    IndicatorId =  grouping.IndicatorId,
                    ComparatorId =  comparatorId,
                    ComparatorMethodId = grouping.ComparatorMethodId,
                    ComparatorConfidence = grouping.ComparatorConfidence,
                    YearRange = grouping.YearRange,
                    Sequence = grouping.Sequence,
                    BaselineYear = grouping.BaselineYear,
                    BaselineQuarter = grouping.BaselineQuarter,
                    BaselineMonth = grouping.BaselineMonth,
                    DataPointYear = grouping.DataPointYear,
                    DataPointQuarter = grouping.DataPointQuarter,
                    DataPointMonth = grouping.DataPointMonth,
                    PolarityId = grouping.PolarityId
                });
            }
        }

        private void UpdateGrouping(Grouping grouping, int oldAreaTypeId, int oldSexId, int oldAgeId)
        {
            if (grouping.ComparatorId == ComparatorIds.Subnational && grouping.AreaTypeId == AreaTypeIds.GoRegion)
            {
                return;
            }

            DeleteIndicatorFromGroupingByAgeSexAndArea(grouping.GroupId, grouping.IndicatorId, oldAgeId, oldSexId,
                oldAreaTypeId);

            foreach (var comparatorId in GetComparatorIds())
            {
                CurrentSession.Save(new Grouping()
                {
                    GroupId = grouping.GroupId,
                    SexId = grouping.SexId,
                    AgeId = grouping.AgeId,
                    AreaTypeId = grouping.AreaTypeId,
                    IndicatorId = grouping.IndicatorId,
                    ComparatorId = comparatorId,
                    ComparatorMethodId = grouping.ComparatorMethodId,
                    ComparatorConfidence = grouping.ComparatorConfidence,
                    YearRange = grouping.YearRange,
                    Sequence = grouping.Sequence,
                    BaselineYear = grouping.BaselineYear,
                    BaselineQuarter = grouping.BaselineQuarter,
                    BaselineMonth = grouping.BaselineMonth,
                    DataPointYear = grouping.DataPointYear,
                    DataPointQuarter = grouping.DataPointQuarter,
                    DataPointMonth = grouping.DataPointMonth,
                    PolarityId = grouping.PolarityId
                });
            }
        }

        private void InsertGrouping(int selectedDomain, int indicatorId, int selectedAreaType, int selectedSex,
            int selectedAgeRange, int selectedComparatorMethods, double selectedComparatorConfidences, int selectedYearRange,
            int selectedPolarityType, int startYear, int endYear, int startQuarterRange, int endQuarterRange, int comparatorId,
            int startMonthRange, int endMonthRange, int indicatorSequence)
        {

            if (comparatorId == ComparatorIds.Subnational && selectedAreaType == AreaTypeIds.GoRegion)
            {
                return;
            }

            CurrentSession.Save(new Grouping()
            {
                GroupId = selectedDomain,
                SexId = selectedSex,
                AgeId = selectedAgeRange,
                AreaTypeId = selectedAreaType,
                IndicatorId = indicatorId,
                ComparatorId = comparatorId,
                ComparatorMethodId = selectedComparatorMethods,
                ComparatorConfidence = selectedComparatorConfidences,
                YearRange = selectedYearRange,
                Sequence = indicatorSequence,
                BaselineYear = startYear,
                BaselineQuarter = startQuarterRange,
                BaselineMonth = startMonthRange,
                DataPointYear = endYear,
                DataPointQuarter = endQuarterRange,
                DataPointMonth = endMonthRange,
                PolarityId = selectedPolarityType
            });
        }

        public int GetNextAvailableIndicatorSequenceNumber(int domainId)
        {
            return
                CurrentSession.CreateCriteria(typeof(Grouping), "grouping")
                    .Add(Restrictions.Eq("grouping.GroupId", domainId))
                    .SetProjection(Projections.Max("Sequence"))
                    .UniqueResult<int>() + 1;
        }

        public bool LogIndicatorMetadataTextPropertyChange(int propertyId, string oldText,
            int indicatorId, int? profileId, string userName, DateTime timestamp)
        {
            CurrentSession.Save(new IndicatorMetadataLog()
            {
                IndicatorId = indicatorId,
                GroupId = profileId,
                PropertyId = propertyId,
                OldText = oldText,
                Timestamp = timestamp,
                UserName = userName
            });
            return true;
        }

        public bool LogAuditChange(string auditMessage, int indicatorId, int? groupId, string userName,
            DateTime timestamp, string auditType)
        {
            CurrentSession.Save(new IndicatorAudit()
            {
                IndicatorId = indicatorId,
                GroupId = groupId.HasValue ? groupId.Value : 0,
                ReasonForChange = auditMessage,
                AuditType = auditType,
                Timestamp = timestamp,
                User = userName
            });
            return true;
        }

        public bool DeleteChangeAudit(int indicatorId)
        {
            var queryString = string.Format(
                @"delete from IndicatorAudit  Where IndicatorId = {0}", indicatorId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
            return true;
        }

        public void MoveIndicatorToDomain(int indicatorId, int fromGroupId, int fromAreaTypeId, int fromSexId,
          int fromAgeId, int toGroupId, int toAreaType, int toSexId, int toAgeId)
        {
            var nextSequenceId = GetNextAvailableIndicatorSequenceNumber(toGroupId);

            try
            {
                transaction = CurrentSession.BeginTransaction();

                foreach (var grouping in GetGroupings(fromGroupId, indicatorId, fromAreaTypeId,
                    fromSexId, fromAgeId))
                {
                    grouping.Sequence = nextSequenceId;
                    grouping.GroupId = toGroupId;
                    grouping.AreaTypeId = toAreaType;
                    grouping.SexId = toSexId;
                    grouping.AgeId = toAgeId;

                    if (!(grouping.AreaTypeId == AreaTypeIds.GoRegion && grouping.ComparatorId == ComparatorIds.Subnational))
                    {
                        CurrentSession.Update(grouping);
                    }
                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void MoveIndicators(int indicatorId, int fromGroupId, int toGroupId,
            int areaTypeId, int sexId, int ageId, string status)
        {
            var nextSequenceId = GetNextAvailableIndicatorSequenceNumber(toGroupId);

            try
            {
                transaction = CurrentSession.BeginTransaction();

                // Update groupings
                var groupings = GetGroupings(fromGroupId, indicatorId, areaTypeId, sexId, ageId);
                foreach (var grouping in groupings)
                {
                    grouping.Sequence = nextSequenceId;
                    grouping.GroupId = toGroupId;
                    grouping.AreaTypeId = areaTypeId;
                    grouping.SexId = sexId;
                    grouping.AgeId = ageId;

                    CurrentSession.Update(grouping);
                }

                // Update indicator metadata status
                var indicatorMetadata = GetIndicatorMetadata(indicatorId);
                indicatorMetadata.Status = status;
                CurrentSession.Update(indicatorMetadata);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void ChangeIndicatorProfile(int indicatorId, int profileId, string status)
        {
            try
            {
                var groupingMetadata = GetGroupingMetadata(profileId);
                if (groupingMetadata != null)
                {
                    // Get the first group id
                    var groupId = groupingMetadata.OrderBy(x => x.Sequence).FirstOrDefault().GroupId;

                    // Queries
                    var updateGroupingQuery = string.Format(@"update Grouping set GroupId = {0} where IndicatorId = {1}",
                        groupId, indicatorId);
                    var updateIndicatorMetadataQuery = string.Format(@"update IndicatorMetadata set OwnerProfileId = {0}, Status = '{1}', DestinationProfileId = -1 where IndicatorId = {2}", profileId, status, indicatorId);

                    // Begin transaction
                    transaction = CurrentSession.BeginTransaction();

                    // Execute queries
                    CurrentSession.CreateQuery(updateGroupingQuery).ExecuteUpdate();
                    CurrentSession.CreateQuery(updateIndicatorMetadataQuery).ExecuteUpdate();

                    // Commit transaction
                    transaction.Commit();
                }

            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public bool IndicatorGroupingsExist(int indicatorId, int domainId, int areaTypeId, int ageId, int sexId)
        {
            var groups = GetGroupings(domainId, indicatorId, areaTypeId, sexId, ageId);

            return groups != null && groups.Any();
        }

        public void CopyIndicatorToDomain(int indicatorId, int fromGroupId, int fromAreaTypeId, int fromSexId,
            int fromAgeId, int toGroupId, int toAreaType, int toSexId, int toAgeId)
        {
            var nextSequenceId = GetNextAvailableIndicatorSequenceNumber(toGroupId);

            try
            {
                transaction = CurrentSession.BeginTransaction();

                var srcGroupings = GetGroupings(fromGroupId, indicatorId,
                    fromAreaTypeId, fromSexId, fromAgeId);

                foreach (var grouping in srcGroupings)
                {
                    var newGrouping = new Grouping()
                    {
                        GroupId = toGroupId,
                        SexId = toSexId,
                        AgeId = toAgeId,
                        AreaTypeId = toAreaType,
                        Sequence = nextSequenceId,
                        IndicatorId = grouping.IndicatorId,
                        ComparatorId = grouping.ComparatorId,
                        ComparatorMethodId = grouping.ComparatorMethodId,
                        ComparatorConfidence = grouping.ComparatorConfidence,
                        YearRange = grouping.YearRange,

                        BaselineYear = grouping.BaselineYear,
                        BaselineQuarter = grouping.BaselineQuarter,
                        BaselineMonth = grouping.BaselineMonth,

                        DataPointYear = grouping.DataPointYear,
                        DataPointQuarter = grouping.DataPointQuarter,
                        DataPointMonth = grouping.DataPointMonth,
                        ComparatorTargetId = grouping.ComparatorTargetId,
                        PolarityId = grouping.PolarityId
                    };

                    if (!(newGrouping.AreaTypeId == AreaTypeIds.GoRegion &&
                          newGrouping.ComparatorId == ComparatorIds.Subnational))
                    {
                        CurrentSession.Save(newGrouping);
                    }


                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void DeleteIndicator(int indicatorId)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                var queryString = string.Format(@"delete from Grouping where IndicatorId = {0}", indicatorId);
                CurrentSession.CreateQuery(queryString).ExecuteUpdate();

                queryString = string.Format(@"delete from IndicatorMetadataTextValue where IndicatorId = {0}",
                    indicatorId);
                CurrentSession.CreateQuery(queryString).ExecuteUpdate();

                queryString = string.Format(@"delete from IndicatorMetadata where IndicatorId = {0}", indicatorId);
                CurrentSession.CreateQuery(queryString).ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void DeleteIndicatorFromGrouping(int? groupId, int? indicatorId, int areaTypeId, int sexId, int ageId)
        {
            var queryString = string.Format(
              @"delete from Grouping where GroupId = '{0}' And IndicatorId = {1} And AreaTypeId = {2} And SexId = {3} And AgeId = {4}",
                     groupId, indicatorId, areaTypeId, sexId, ageId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
        }

        public void DeleteOverriddenMetadataTextValues(int? indicatorId, int profileId)
        {
            var queryString = string.Format(
              @"delete from IndicatorMetadataTextValue where IndicatorId = {0} And ProfileId = {1}", indicatorId, profileId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
        }

        public void UnassignIndicatorFromGrouping(int groupId, int? indicatorId, int areaTypeId, int sexId, int ageId)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                foreach (var grouping in GetGroupings(groupId, indicatorId.Value, areaTypeId, sexId, ageId))
                {
                    grouping.GroupId = GroupIds.UnassignedIndicators;

                    CurrentSession.Update(grouping);
                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public string GetDomainName(int groupId, int domainSequence)
        {
            IQuery q = CurrentSession.CreateQuery("select g.GroupName from GroupingMetadata g where g.GroupId = :groupId and g.Sequence = :domainSequence");
            q.SetParameter("groupId", groupId);
            q.SetParameter("domainSequence", domainSequence);
            return q.UniqueResult<string>();
        }

        public IList<GroupingPlusName> GetGroupingPlusNames(int indicatorId, int? selectedDomainId,
            int areaTypeId, int profileId, bool areIndicatorNamesDisplayedWithNumbers)
        {
            var groupings = CurrentSession.GetNamedQuery("GetGroupingPlusNames")
                .SetParameter("IndicatorId", indicatorId)
                .SetParameter("SelectedDomainId", selectedDomainId)
                .SetParameter("ProfileId", profileId)
                .SetParameter("AreaTypeId", areaTypeId)
                .SetResultTransformer(Transformers.AliasToBean<GroupingPlusName>())
                .List<GroupingPlusName>();

            var groupingPlusNamesWithoutOverriddenMetadata = new List<GroupingPlusName>();
            var groupingPlusNamesWithOverriddenMetadata = new List<GroupingPlusName>();
            var indicatorName = string.Empty;
            var indicatorNumber = string.Empty;

            if (groupings != null)
            {
                indicatorName = GetIndicatorName(groupings);
                indicatorNumber = GetIndicatorNumber(groupings);
            }

            if (areIndicatorNamesDisplayedWithNumbers)
            {
                if (!string.IsNullOrWhiteSpace(indicatorNumber)
                    && indicatorNumber.Trim().Length > 0
                    && !string.IsNullOrWhiteSpace(indicatorName))
                {
                    indicatorName = String.Format("{0} - {1}", indicatorNumber, indicatorName);
                }
            }

            foreach (var grouping in groupings)
            {
                grouping.IndicatorName = indicatorName;

                var isOveriddingMetadata = grouping.ProfileId.HasValue;

                if (isOveriddingMetadata)
                {
                    groupingPlusNamesWithOverriddenMetadata.Add(grouping);
                }
                else
                {
                    groupingPlusNamesWithoutOverriddenMetadata.Add(grouping);
                }
            }

            if (groupingPlusNamesWithOverriddenMetadata.Any())
            {
                foreach (var groupingPlusName in groupingPlusNamesWithOverriddenMetadata)
                {
                    // Is the indicator name not overidden?
                    if (string.IsNullOrEmpty(groupingPlusName.IndicatorName))
                    {
                        // Assign base indicator name
                        groupingPlusName.IndicatorName = indicatorName;
                    }
                }

                return groupingPlusNamesWithOverriddenMetadata;
            }

            return groupingPlusNamesWithoutOverriddenMetadata;
        }

        public IList<GroupingSubheading> GetGroupingSubheadingsByGroupIds(IList<int> groupIds)
        {
            return CurrentSession.CreateCriteria<GroupingSubheading>()
                .Add(Restrictions.In("GroupId", groupIds.ToList()))
                .List<GroupingSubheading>();
        }

        public IList<GroupingSubheading> GetGroupingSubheadings(int areaTypeId, int groupId)
        {
            IQuery q = CurrentSession.CreateQuery("from GroupingSubheading g where g.AreaTypeId = :areaTypeId and g.GroupId = :groupId");
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetParameter("groupId", groupId);
            return q.List<GroupingSubheading>();
        }

        public GroupingSubheading GetGroupingSubheading(int subheadingId)
        {
            IQuery q = CurrentSession.CreateQuery("from GroupingSubheading g where g.SubheadingId = :subheadingId");
            q.SetParameter("subheadingId", subheadingId);
            return q.UniqueResult<GroupingSubheading>();
        }

        public void SaveGroupingSubheading(GroupingSubheading groupingSubheading)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                // Save new 
                CurrentSession.GetNamedQuery("Insert_GroupingSubheading")
                    .SetParameter("GroupId", groupingSubheading.GroupId)
                    .SetParameter("AreaTypeId", groupingSubheading.AreaTypeId)
                    .SetParameter("Subheading", groupingSubheading.Subheading)
                    .SetParameter("Sequence", groupingSubheading.Sequence)
                    .ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void UpdateGroupingSubheading(GroupingSubheading groupingSubheading)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.GetNamedQuery("Update_GroupingSubheading")
                    .SetParameter("GroupId", groupingSubheading.GroupId)
                    .SetParameter("AreaTypeId", groupingSubheading.AreaTypeId)
                    .SetParameter("Subheading", groupingSubheading.Subheading)
                    .SetParameter("Sequence", groupingSubheading.Sequence)
                    .SetParameter("SubheadingId", groupingSubheading.SubheadingId)
                    .ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void DeleteGroupingSubheading(int subheadingId)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                int deleteCount = CurrentSession.GetNamedQuery("Delete_GroupingSubheading")
                    .SetParameter("SubheadingId", subheadingId)
                    .ExecuteUpdate();

                if (deleteCount == 0)
                {
                    throw new Exception(String.Format("Unable to delete the grouping subheading with id {0}", subheadingId));
                }

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public ProfileDetails GetProfileDetailsById(int id)
        {
            var profileDetail = CurrentSession.QueryOver<ProfileDetails>()
                .Where(ugp => ugp.Id == id)
                .SingleOrDefault();

            return profileDetail;
        }

        public ProfileDetails GetProfileDetailsByUrlKey(string urlKey)
        {
            var profileDetail = CurrentSession.QueryOver<ProfileDetails>()
                .Where(ugp => ugp.UrlKey == urlKey)
                .SingleOrDefault();

            return profileDetail;
        }

        public void LogIndicatorMetadataReviewAudit(IndicatorMetadataReviewAudit indicatorMetadataReviewAudit)
        {
            CurrentSession.Save(indicatorMetadataReviewAudit);
        }

        private void DeleteIndicatorFromGroupingByAgeSexAndArea(int groupId, int indicatorId, int ageId, int sexId,
            int areaTypeId)
        {
            var query = string.Format(
                @"Delete from Grouping Where GroupId = '{0}' And IndicatorId = {1} And AgeId = {2} And SexId = {3} And AreaTypeId = {4}", groupId, indicatorId, ageId, sexId, areaTypeId);

            CurrentSession.CreateQuery(query).ExecuteUpdate();
        }

        private static IEnumerable<int> GetComparatorIds()
        {
            var comparatorIds = new List<int>
            {
                ComparatorIds.Subnational,
                ComparatorIds.National
            };

            return comparatorIds;
        }

        private IEnumerable<Grouping> GetGroupings(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            var query = string.Format(
                    "from Grouping where GroupId = {0} And IndicatorId = {1} And AreaTypeId = {2} And SexId = {3} And AgeId = {4}",
                    groupId, indicatorId, areaTypeId, sexId, ageId
                    );

            return CurrentSession.CreateQuery(query).List<Grouping>();
        }

        private IEnumerable<GroupingMetadata> GetGroupingMetadata(int profileId)
        {
            return CurrentSession.CreateCriteria<GroupingMetadata>()
                .Add(Restrictions.Eq("ProfileId", profileId))
                .List<GroupingMetadata>();
        }

        private string GetIndicatorName(IList<GroupingPlusName> groupings)
        {
            var grouping = groupings.FirstOrDefault(x => x.IndicatorName != null);
            if (grouping != null)
            {
                return grouping.IndicatorName;
            }

            return string.Empty;
        }

        private string GetIndicatorNumber(IList<GroupingPlusName> groupings)
        {
            var grouping = groupings.FirstOrDefault(x => x.IndicatorNumber != null);
            if (grouping != null)
            {
                return grouping.IndicatorNumber;
            }

            return string.Empty;
        }
    }
}