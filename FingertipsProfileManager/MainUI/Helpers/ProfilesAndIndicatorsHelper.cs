using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.DataAudit;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Fpm.MainUI.Helpers
{
    public class ProfilesAndIndicatorsHelper
    {
        private readonly IProfilesReader _reader;
        private readonly IProfilesWriter _writer;
        private readonly IProfileRepository _profileRepository;

        private readonly string _userName;

        public ProfilesAndIndicatorsHelper(IProfilesReader reader, IProfilesWriter writer,
            IProfileRepository profileRepository)
        {
            _reader = reader;
            _writer = writer;
            _profileRepository = profileRepository;

            _userName = UserDetails.CurrentUser().Name;
        }

        public int GetSelectedGroupIdUsingProfileKeyDomainAndAreaTypeId(string profileUrlKey, int domainNumber, int areaTypeId)
        {
            var model = new ProfileMembers
            {
                UrlKey = profileUrlKey,
                Profile = CommonUtilities.GetProfile(profileUrlKey, domainNumber, areaTypeId, _profileRepository)
            };

            IList<GroupingMetadata> groupingMetadatas = model.Profile.GroupingMetadatas;

            return groupingMetadatas.FirstOrDefault(x => x.Sequence == domainNumber).GroupId;
        }

        public void UpdateGroupings(IList<GroupingPlusName> groupingPlusNames, int groupId)
        {
            foreach (var groupingPlusName in groupingPlusNames)
            {
                var groupings = _reader.GetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexIdAndAgeId(groupId, groupingPlusName.AreaTypeId,
                    groupingPlusName.IndicatorId, groupingPlusName.SexId, groupingPlusName.AgeId);

                foreach (var grouping in groupings)
                {
                    grouping.Sequence = groupingPlusName.Sequence;
                }

                _writer.UpdateGroupingList(groupings);
            }
        }

        public Dictionary<string, object> GetFilters(BrowseDataViewModel model)
        {
            var filters = new Dictionary<string, object>();

            if (model.AgeId > UIHelper.ShowAll) filters.Add(CoreDataFilters.AgeId, model.AgeId);
            if (model.SexId > UIHelper.ShowAll) filters.Add(CoreDataFilters.SexId, model.SexId);
            if (model.AreaTypeId > 0) filters.Add(CoreDataFilters.AreaTypeId, model.AreaTypeId);
            if (model.CategoryTypeId > 0) filters.Add(CoreDataFilters.CategoryTypeId, model.CategoryTypeId);
            if (model.Month > 0) filters.Add(CoreDataFilters.Month, model.Month);
            if (model.Year > 0) filters.Add(CoreDataFilters.Year, model.Year);
            if (model.Quarter > 0) filters.Add(CoreDataFilters.Quarter, model.Quarter);
            if (model.YearRange > 0) filters.Add(CoreDataFilters.YearRange, model.YearRange);

            // Add area code
            var areaCode = model.AreaCode;
            if (string.IsNullOrWhiteSpace(areaCode) == false)
            {
                filters.Add(CoreDataFilters.AreaCode, model.AreaCode.Trim());
            }

            return filters;
        }

        public List<UserGroupPermissions> GetUserGroupPermissions()
        {
            var fpmUser = _reader.GetUserByUserName(_userName);
            List<UserGroupPermissions> userPermissions = fpmUser != null
                ? CommonUtilities.GetUserGroupPermissionsByUserId(fpmUser.Id).ToList()
                : new List<UserGroupPermissions>();
            return userPermissions;
        }

        public DataChange GetDataChangeAudit(int indicatorId)
        {
            try
            {
                var url = ApplicationConfiguration.CoreWsUrl + "api/data_changes?indicator_id=" + indicatorId;
                string json;
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(url);
                }

                var dataChange = JsonConvert.DeserializeObject<DataChange>(json);
                return dataChange;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void MoveIndicatorInDb(int fromGroupId, int toGroupId, int areaTypeId, int indicatorId, int sexId, int ageId, string status)
        {
            // Move indicators
            _profileRepository.MoveIndicators(indicatorId, fromGroupId, toGroupId, areaTypeId, sexId, ageId, status);

            // Add entry to indicator metadata review audit
            LogIndicatorMetadataReviewAudit(indicatorId, toGroupId);

            // Log audit change
            LogAuditChange(fromGroupId, toGroupId, indicatorId);

            // Reorder indicator sequence
            _writer.ReorderIndicatorSequence(toGroupId, areaTypeId);
        }

        public void LogIndicatorMetadataReviewAudit(int indicatorId, int toGroupId)
        {
            var auditTypeId = -1;
            var notes = string.Empty;

            switch (toGroupId)
            {
                case GroupIds.UnderReview:
                    auditTypeId = IndicatorMetadataReviewAuditType.SubmittedForReview;
                    notes = IndicatorMetadataReviewAuditTypeNotes.SubmittedForReview;
                    break;

                case GroupIds.InDevelopment:
                    auditTypeId = IndicatorMetadataReviewAuditType.BackToInDevelopment;
                    notes = IndicatorMetadataReviewAuditTypeNotes.BackToInDevelopment;
                    break;

                case GroupIds.AwaitingRevision:
                    auditTypeId = IndicatorMetadataReviewAuditType.AwaitingRevision;
                    notes = IndicatorMetadataReviewAuditTypeNotes.AwaitingRevision;
                    break;

                default:
                    throw new Exception(string.Format("IndicatorId: {0}, indicator metadata review audit is not applicable for group id {1}", indicatorId, toGroupId));
            }

            var indicatorMetadataReviewAudit = new IndicatorMetadataReviewAudit()
            {
                IndicatorId = indicatorId,
                AuditTypeId = auditTypeId,
                Notes = notes,
                Timestamp = DateTime.Now,
                UserId = UserDetails.CurrentUser().FpmUser.Id
            };

            _profileRepository.LogIndicatorMetadataReviewAudit(indicatorMetadataReviewAudit);
        }

        public void LogAuditChange(int fromGroupId, int toGroupId, int indicatorId)
        {
            var auditMessage = string.Format("Indicator {0} moved from group {1} to group {2}", indicatorId,
                fromGroupId, toGroupId);

            _profileRepository.LogAuditChange(auditMessage, indicatorId, null, _userName, DateTime.Now,
                CommonUtilities.AuditType.Move.ToString());
        }
    }
}