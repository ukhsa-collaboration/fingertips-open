using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Logging;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Helpers;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;

namespace Fpm.ProfileData.Repositories
{
    public class ProfileRepository : RepositoryBase
    {
        // poor man injection, should be removed when we use DI containers
        public ProfileRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public ProfileRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
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

        public void CreateIndicator(IEnumerable<IndicatorMetadataTextProperty> allPropertiesToAdd, int indicatorId)
        {
            var indicatorMetaDataTextValue = new IndicatorMetadataTextValue();

            try
            {
                transaction = CurrentSession.BeginTransaction();

                allPropertiesToAdd.ForEach(
                   p => indicatorMetaDataTextValue.SetPropertyValue(p.ColumnName, p.Text));

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
            var indicatorMetaDataTextValue = new IndicatorMetadataTextValue()
            {
                IndicatorId = indicatorId,
                ProfileId = profileId,
            };

            indicatorMetaDataTextValue.SetPropertyValue(property.ColumnName, text);

            CurrentSession.Save(indicatorMetaDataTextValue);
        }

        public bool DoesOverriddenIndicatorMetaDataRecordAlreadyExist(int indicatorId, int? profileId)
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

        public bool UpdateProfileCollection(int profileCollectionId, string assignedProfilesToUpdate, string collectionNameToUpdate, string collectionSkinTitleToUpdate)
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
                        var assignedProfileId = assignedProfile.Split('~')[0];
                        var showDomain = assignedProfile.Split('~')[1] == "true";

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
            var groupMatch = profileId.HasValue ? "=" + profileId.Value : "IS NULL";

            var queryString = string.Format(
                @"update IndicatorMetadataTextValue set {0} = :text where IndicatorID = {1} AND ProfileId {2}",
                     property.ColumnName, indicatorId, groupMatch);

            var updateSession = CurrentSession.CreateQuery(queryString);

            updateSession.SetParameter("text", text);

            updateSession.ExecuteUpdate();
        }

        public bool CreateGroupingAndMetaData(int selectedProfileId, int selectedDomain, int nextIndicatorId,
            int selectedAreaType, int selectedSex, int selectedAgeRange, int selectedComparator, int selectedComparatorMethods,
            double selectedComparatorConfidences, int selectedYearType, int selectedYearRange, int selectedValueType,
            int selectedCiMethodType, double selectedCiConfidenceLevel, int selectedPolarityType, int selectedUnitType,
            int selectedDenominatorType, int startYear, int endYear, int startQuarterRange, int endQuarterRange,
            int startMonthRange, int endMonthRange, int? selectedDecimalPlaces, int? selectedTargetId)
        {
            var indicatorMetaData = new IndicatorMetadata()
            {
                IndicatorId = nextIndicatorId,
                DenominatorTypeId = selectedDenominatorType,
                CIMethodId = selectedCiMethodType,
                ValueTypeId = selectedValueType,
                UnitId = selectedUnitType,
                YearTypeId = selectedYearType,
                ConfidenceLevel = selectedCiConfidenceLevel,
                OwnerProfileId = selectedProfileId,
                DecimalPlacesDisplayed = selectedDecimalPlaces,
                TargetId = selectedTargetId,
                DateEntered = DateTime.Now
            };

            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(indicatorMetaData);

                var nextAvailableIndicatorSequenceNumber = GetNextAvailableIndicatorSequenceNumber(selectedDomain);

                foreach (var comparatorId in GetComparatorIds(selectedComparator))
                {
                    InsertGrouping(selectedDomain, nextIndicatorId, selectedAreaType, selectedSex, selectedAgeRange,
                        selectedComparatorMethods, selectedComparatorConfidences, selectedYearRange,
                        selectedPolarityType, startYear, endYear, startQuarterRange, endQuarterRange,
                        comparatorId, startMonthRange, endMonthRange, nextAvailableIndicatorSequenceNumber);
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

        public bool UpdateGroupingAndMetaData(int selectedDomain, int indicatorId, int selectedAreaType, int selectedSex,
            int selectedAgeRange, int selectedComparator, int selectedComparatorMethods, string selectedCiComparatorConfidence,
            double selectedComparatorConfidences, int selectedYearType, int selectedYearRange, int selectedValueType,
            int selectedCiMethodType, int selectedPolarityType, int selectedUnitType, int selectedDenominatorType,
            int startYear, int endYear, int startQuarterRange, int endQuarterRange, int startMonthRange, int endMonthRange,
            int indicatorSequence, int currentAgeId, int currentSexId, int currentAreaTypeId, IEnumerable<string> userAudit,
            string user, string auditType, int? selectedDecimalPlaces, int? targetId, bool alwaysShowSpineChart)
        {
            try
            {
                var indicatorMetaData = GetIndicatorMetadata(indicatorId);

                indicatorMetaData.IndicatorId = indicatorId;
                indicatorMetaData.DenominatorTypeId = selectedDenominatorType;
                indicatorMetaData.CIMethodId = selectedCiMethodType;
                indicatorMetaData.ValueTypeId = selectedValueType;
                indicatorMetaData.UnitId = selectedUnitType;
                indicatorMetaData.ConfidenceLevel = Convert.ToDouble(selectedCiComparatorConfidence);
                indicatorMetaData.YearTypeId = selectedYearType;
                indicatorMetaData.DecimalPlacesDisplayed = selectedDecimalPlaces;
                indicatorMetaData.TargetId = targetId;
                indicatorMetaData.AlwaysShowSpineChart = alwaysShowSpineChart;

                transaction = CurrentSession.BeginTransaction();

                CurrentSession.SaveOrUpdate(indicatorMetaData);

                // Delete the existing grouping records for this indicator/domain 
                DeleteIndicatorFromGroupingByAgeSexAndArea(selectedDomain, indicatorId, currentAgeId, currentSexId,
                    currentAreaTypeId);
            
                foreach (var comparatorId in GetComparatorIds(selectedComparator))
                {
                         InsertGrouping(selectedDomain, indicatorId, selectedAreaType, selectedSex, selectedAgeRange,
                           selectedComparatorMethods, selectedComparatorConfidences, selectedYearRange,
                           selectedPolarityType, startYear, endYear, startQuarterRange, endQuarterRange,
                           comparatorId, startMonthRange, endMonthRange, indicatorSequence);
                }

                // Update the FPM Audit Log
                foreach (var userChange in userAudit.Where(userChange => !string.IsNullOrEmpty(userChange)))
                {
                    LogAuditChange(userChange.Trim(), indicatorId, selectedDomain, user, DateTime.Now, auditType);
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

        public bool LogPropertyChange(int propertyId, string oldText, int indicatorId, int? groupId,
            string userName, DateTime timestamp)
        {
            CurrentSession.Save(new IndicatorMetadataLog()
            {
                IndicatorId = indicatorId,
                GroupId = groupId.HasValue ? groupId.Value : 0,
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

                foreach (var grouping in GetGroupings(fromGroupId, indicatorId, 
                    fromAreaTypeId, fromSexId, fromAgeId))
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

        public void DeleteIndicatorFromGrouping(int? groupId, int? indicatorId, int areaTypeId, int sexId, int ageId)
        {
            var queryString = string.Format(
              @"delete from Grouping where GroupId = '{0}' And IndicatorId = {1} And AreaTypeId = {2} And SexId = {3} And AgeId = {4}",
                     groupId, indicatorId, areaTypeId, sexId, ageId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
        }

        public void DeleteOverriddenMetaDataTextValues(int? indicatorId, int profileId)
        {
            var queryString = string.Format(
              @"delete from IndicatorMetadataTextValue where IndicatorId = {0} And ProfileId = {1}", indicatorId, profileId);

            CurrentSession.CreateQuery(queryString).ExecuteUpdate();
        }

        public void ArchiveIndicatorFromGrouping(int groupId, int? indicatorId, int areaTypeId, int sexId, int ageId)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                foreach (var grouping in GetGroupings(groupId, indicatorId.Value, areaTypeId, sexId, ageId))
                {
                    grouping.GroupId = GroupIds.ArchivedIndicators;

                    CurrentSession.Update(grouping);
                }
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public IList<GroupingPlusName> GetGroupingPlusNames(int indicatorId, int? selectedDomainId, int areaTypeId, int profileId)
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

            foreach (var grouping in groupings)
            {
                var isOveriddingMetadata = grouping.ProfileId.HasValue;

                if (isOveriddingMetadata)
                {
                    groupingPlusNamesWithOverriddenMetadata.Add(grouping);
                }
                else
                {
                    indicatorName = grouping.IndicatorName;
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

        private void DeleteIndicatorFromGroupingByAgeSexAndArea(int? groupId,
            int? indicatorId, int? ageId, int? sexId, int? areaTypeId)
        {
            var query = string.Format(
                @"Delete from Grouping Where GroupId = '{0}' And IndicatorId = {1} And AgeId = {2} And SexId = {3} And AreaTypeId = {4}", groupId, indicatorId, ageId, sexId, areaTypeId);

            CurrentSession.CreateQuery(query).ExecuteUpdate();
        }

        private static IEnumerable<int> GetComparatorIds(int selectedComparator)
        {
            var comparatorIds = new List<int>();
            if (selectedComparator == ComparatorIds.NationalAndSubnational)
            {
                comparatorIds.Add(ComparatorIds.Subnational);
                comparatorIds.Add(ComparatorIds.National);
            }
            else
            {
                comparatorIds.Add(selectedComparator);
            }
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
    }
}