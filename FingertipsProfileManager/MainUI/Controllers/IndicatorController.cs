using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels.Indicator;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("indicators")]
    public class IndicatorController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ICoreDataRepository _coreDataRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailRepository;

        private readonly IProfilesReader _reader;
        private UserDetails _user;

        public IndicatorController(IProfileRepository profileRepository, ICoreDataRepository coreDataRepository,
            IUserRepository userRepository, IEmailRepository emailRepository, IProfilesReader reader)
        {
            _profileRepository = profileRepository;
            _coreDataRepository = coreDataRepository;
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _reader = reader;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _user = UserDetails.CurrentUser();
        }

        [HttpGet]
        [Route("create-indicator")]
        public ActionResult CreateIndicator()
        {
            var profiles = GetOrderedListOfProfiles();
            var domains = GetOrderedListOfDomains(profiles.FirstOrDefault());

            ViewBag.Profiles = profiles;
            ViewBag.Domains = domains;

            var indicatorMetadata = new IndicatorMetadata
            {
                Status = IndicatorStatus.InDevelopment,
                DestinationProfileId = -1
            };

            var indicatorViewModel = new IndicatorViewModel()
            {
                IndicatorMetadata = indicatorMetadata,
                IndicatorMetadataTextValue = new IndicatorMetadataTextValue(),
                IndicatorMetadataTextProperties = _reader.GetIndicatorMetadataTextProperties().ToList(),
                IndicatorMetadataReviewAudits = new List<IndicatorMetadataReviewAudit>(),
                FpmUsers = _userRepository.GetAllFpmUsers().ToList(),
                IsEditAction = false,
                Grouping = new Grouping()
            };


            return View("CreateIndicator", indicatorViewModel);
        }

        [HttpGet]
        [Route("edit-indicator")]
        public ActionResult EditIndicator(string urlKey, int areaType, int domain, int indicatorId, int sexId, int ageId)
        {
            var profile = new ProfileBuilder(_reader, _profileRepository).Build(urlKey, domain, areaType);
            var indicatorMetadata = _reader.GetIndicatorMetadata(indicatorId);

            IList<IndicatorMetadataTextValue> indicatorMetadataTextValues =
                _reader.GetIndicatorMetadataTextValuesByIndicatorIdsAndProfileId(new List<int>() { indicatorId }, profile.Id);

            var indicatorMetadataTextValue = indicatorMetadataTextValues[0];

            var groupId = GetGroupId(domain, profile);
            var groupings = _reader.GetGroupings(groupId).Where(g =>
                g.IndicatorId == indicatorId && g.GroupId == groupId && g.AreaTypeId == areaType && g.SexId == sexId &&
                g.AgeId == ageId).ToList();

            // Get the time series
            var timeSeries = GetTimeSeries(groupings[0]);
            groupings[0].TimeSeries = timeSeries;

            // Get indicator metadata text properties
            var indicatorMetadataTextProperties = _reader.GetIndicatorMetadataTextProperties().ToList();

            // Get indicator metadata review audits
            var indicatorMetadataReviewAudits = _reader.GetIndicatorMetadataReviewAudits(indicatorId).ToList();

            var partitionAgeIds = indicatorMetadata.PartitionAgeIds == null
                ? new List<string>()
                : indicatorMetadata.PartitionAgeIds.Split(',').ToList();

            var partitionSexIds = indicatorMetadata.PartitionSexIds == null
                ? new List<string>()
                : indicatorMetadata.PartitionSexIds.Split(',').ToList();

            var partitionAreaTypeIds = indicatorMetadata.PartitionAreaTypeIds == null
                ? new List<string>()
                : indicatorMetadata.PartitionAreaTypeIds.Split(',').ToList();

            // Destination profile
            var destinationProfileName = string.Empty;
            var destinationProfileId = indicatorMetadata.DestinationProfileId;
            if (destinationProfileId != -1)
            {
                destinationProfileName = _reader.GetProfileDetailsByProfileId(destinationProfileId).Name;
            }

            IEnumerable <UserGroupPermissions> userPermissions = CommonUtilities.GetUserGroupPermissionsByUserId(_user.Id);
            var userGroupPermissions = userPermissions.FirstOrDefault(x => x.ProfileId == profile.Id);

            var indicatorViewModel = new IndicatorViewModel()
            {
                IndicatorMetadata = indicatorMetadata,
                IndicatorMetadataTextValue = indicatorMetadataTextValue,
                IndicatorMetadataTextProperties = indicatorMetadataTextProperties,
                IndicatorMetadataReviewAudits = indicatorMetadataReviewAudits,
                Grouping = groupings[0],
                UrlKey = urlKey,
                Profile = profile,
                PartitionAgeIds = partitionAgeIds,
                PartitionSexIds = partitionSexIds,
                PartitionAreaTypeIds = partitionAreaTypeIds,
                UserGroupPermissions = userGroupPermissions,
                DomainSequence = domain,
                FpmUsers = _userRepository.GetAllFpmUsers().ToList(),
                DestinationProfileName = destinationProfileName,
                AreaTypeId = groupings[0].AreaTypeId,
                SexId = groupings[0].SexId,
                AgeId = groupings[0].AgeId,
                IsEditAction = true
            };

            // Enable detection of user change
            indicatorViewModel.IndicatorMetadata.NextReviewTimestampInitialValue =
                indicatorViewModel.IndicatorMetadata.NextReviewTimestamp;

            var profiles = GetOrderedListOfProfiles();
            var domains = GetOrderedListOfDomains(profiles.FirstOrDefault());

            ViewBag.Profiles = profiles;
            ViewBag.Domains = domains;

            return View("EditIndicator", indicatorViewModel);
        }

        [HttpPost]
        [Route("save-indicator")]
        public ActionResult SaveIndicator(IndicatorViewModel indicatorViewModel)
        {
            var indicatorId = 0;
            var profileId = 0;
            var groupId = 0;
            var profileUrlKey = "";
            var isNewIndicator = false;

            var grouping = indicatorViewModel.Grouping;
            switch (grouping.TimeSeries)
            {
                case TimeSeries.Annual:
                    grouping.BaselineMonth = -1;
                    grouping.DataPointMonth = -1;
                    grouping.BaselineQuarter = -1;
                    grouping.DataPointQuarter = -1;
                    break;

                case TimeSeries.Quarter:
                    grouping.BaselineMonth = -1;
                    grouping.DataPointMonth = -1;
                    break;

                case TimeSeries.Month:
                    grouping.BaselineQuarter = -1;
                    grouping.DataPointQuarter = -1;
                    break;
            }

            if (indicatorViewModel.CopyToProfileUrlKey != null)
            {
                var destinationProfileUrlKey = indicatorViewModel.CopyToProfileUrlKey;
                indicatorViewModel.IndicatorMetadata.DestinationProfileId = _reader.GetProfileDetails(destinationProfileUrlKey).Id;

                // Creating new indicator by coping the existing indicator metadata
                // must undergo review and approve process.
                profileId = ProfileIds.IndicatorsForReview;
                groupId = GroupIds.InDevelopment;

                indicatorId = SaveIndicator(indicatorViewModel, profileId, groupId);
                isNewIndicator = true;
            }
            else
            {
                if (indicatorViewModel.IsEditAction)
                {
                    profileUrlKey = indicatorViewModel.UrlKey;
                    profileId = _reader.GetProfileDetails(profileUrlKey).Id;
                    indicatorId = UpdateIndicator(indicatorViewModel, profileId);
                }
                else
                {
                    profileUrlKey = UrlKeys.IndicatorsForReview;
                    profileId = ProfileIds.IndicatorsForReview;
                    groupId = GroupIds.InDevelopment;
                    indicatorViewModel.IndicatorMetadata.DestinationProfileId =
                        _reader.GetProfileDetails(indicatorViewModel.DestinationProfileUrlKey).Id;
                    indicatorId = SaveIndicator(indicatorViewModel, profileId, groupId);
                    isNewIndicator = true;
                }
            }

            return RedirectToAction("ListIndicatorsInProfileSpecific", "ProfilesAndIndicators",
                new
                {
                    ProfileKey = profileUrlKey,
                    DomainSequence = indicatorViewModel.DomainSequence,
                    SelectedAreaTypeId = indicatorViewModel.Grouping.AreaTypeId,
                    NewIndicatorId = indicatorId,
                    IsNewIndicator = isNewIndicator
                });
        }

        private int UpdateIndicator(IndicatorViewModel indicatorViewModel, int profileId)
        {
            var updatedIndicatorId = indicatorViewModel.IndicatorMetadata.IndicatorId;

            // Assign indicator metadata properties
            var metadata = indicatorViewModel.IndicatorMetadata;
            metadata.IndicatorId = updatedIndicatorId;
            metadata.PartitionAgeIds = JoinStringList(indicatorViewModel.PartitionAgeIds);
            metadata.PartitionSexIds = JoinStringList(indicatorViewModel.PartitionSexIds);
            metadata.PartitionAreaTypeIds = JoinStringList(indicatorViewModel.PartitionAreaTypeIds);

            // Assign indicator metadata text value properties
            var metadataTextValue = indicatorViewModel.IndicatorMetadataTextValue;
            metadataTextValue.IndicatorId = updatedIndicatorId;

            // Assign grouping properties
            indicatorViewModel.Grouping.IndicatorId = updatedIndicatorId;

            // If should averages be calculated flag is not set then
            // delete core data sets for this indicator with value note 506
            if (!indicatorViewModel.IndicatorMetadata.ShouldAveragesBeCalculated)
            {
                _coreDataRepository.DeleteCoreDataSet(indicatorViewModel.Grouping.IndicatorId, 
                    ValueNoteIds.AggregatedFromAllKnownLowerGeographyValuesByFingertips);
            }

            // Trim indicator metadata text values
            TrimIndicatorMetadataTextValue(metadataTextValue);

            // Update indicator
            _profileRepository.UpdateGroupingAndMetadata(metadata, metadataTextValue, indicatorViewModel.Grouping,
                indicatorViewModel.AreaTypeId, indicatorViewModel.SexId, indicatorViewModel.AgeId, _user.Name);

            // Log indicator metadata review audit
            LogIndicatorMetadataReviewAudit(indicatorViewModel);

            // Return the updated indicator id
            return updatedIndicatorId;
        }

        private int SaveIndicator(IndicatorViewModel indicatorViewModel, int profileId, int groupId)
        {
            var nextIndicatorId = _profileRepository.GetNextIndicatorId();
            var nextAvailableIndicatorSequenceNumber = _profileRepository.GetNextAvailableIndicatorSequenceNumber(groupId);

            // Assign indicator metadata properties
            var metadata = indicatorViewModel.IndicatorMetadata;
            metadata.IndicatorId = nextIndicatorId;
            metadata.OwnerProfileId = profileId;
            metadata.DateEntered = DateTime.Now;
            metadata.PartitionAgeIds = JoinStringList(indicatorViewModel.PartitionAgeIds);
            metadata.PartitionSexIds = JoinStringList(indicatorViewModel.PartitionSexIds);
            metadata.PartitionAreaTypeIds = JoinStringList(indicatorViewModel.PartitionAreaTypeIds);

            // Assign indicator metadata text value properties
            var metadataTextValue = indicatorViewModel.IndicatorMetadataTextValue;
            metadataTextValue.IndicatorId = nextIndicatorId;

            // Assign grouping properties
            indicatorViewModel.Grouping.GroupId = groupId;
            indicatorViewModel.Grouping.IndicatorId = nextIndicatorId;
            indicatorViewModel.Grouping.Sequence = nextAvailableIndicatorSequenceNumber;

            // Trim indicator metadata text values
            TrimIndicatorMetadataTextValue(metadataTextValue);

            // Create new indicator
            _profileRepository.CreateGroupingAndMetadata(metadata, metadataTextValue, 
                indicatorViewModel.Grouping, _user.Name);

            // Log indicator metadata review audit
            LogIndicatorMetadataReviewAudit(indicatorViewModel);

            // Email to the user
            var sex = _reader.GetAllSexes().FirstOrDefault(x => x.SexID == indicatorViewModel.Grouping.SexId);
            var age = _reader.GetAllAges().FirstOrDefault(x => x.AgeID == indicatorViewModel.Grouping.AgeId);
            var indicatorName = string.Format("{0} ({1}) ({2})", metadataTextValue.Name, sex.Description, age.Description);
            var urlReferrer = Request.UrlReferrer;
            var indicatorUrl = CommonUtilities.GetIndicatorUrl(urlReferrer.Scheme, urlReferrer.Authority, "/profile/indicators/specific", UrlKeys.IndicatorsForReview, 1, indicatorViewModel.Grouping.AreaTypeId);

            EmailHelper emailHelper = new EmailHelper(_emailRepository, NotifyEmailTemplates.IndicatorCreated, nextIndicatorId, indicatorName, indicatorUrl);
            emailHelper.SendEmail();

            // Return the newly created indicator id
            return nextIndicatorId;
        }

        private string JoinStringList(IList<string> list)
        {
            return list == null
                ? string.Empty
                : string.Join(",", list);
        }

        private void TrimIndicatorMetadataTextValue(
            IndicatorMetadataTextValue imtv)
        {
            var stringProperties = imtv.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(imtv, null);
                if (currentValue != null)
                {
                    stringProperty.SetValue(imtv, currentValue.Trim(), null);
                }
            }
        }

        private void LogIndicatorMetadataReviewAudit(IndicatorViewModel indicatorViewModel)
        {
            // If user is not reviewer then will not be able to save notes
            if (indicatorViewModel.IndicatorMetadataReviewAudit == null) return;

            var notes = indicatorViewModel.IndicatorMetadataReviewAudit.Notes;
            var nextReviewTimestamp = indicatorViewModel.IndicatorMetadata.NextReviewTimestamp;
            var indicatorId = indicatorViewModel.Grouping.IndicatorId;

            var hasTimestampChanged = HasTimestampChanged(indicatorViewModel);

            // Only log if have either notes or timestamp
            if (string.IsNullOrWhiteSpace(notes) == false || hasTimestampChanged)
            {
                var message = GetAuditMessage(notes, nextReviewTimestamp, hasTimestampChanged);

                var reviewAudit = new IndicatorMetadataReviewAudit()
                {
                    IndicatorId = indicatorId,
                    AuditTypeId = IndicatorMetadataReviewAuditType.ReviewerNote,
                    Notes = message,
                    Timestamp = DateTime.Now,
                    UserId = _user.FpmUser.Id
                };
                _profileRepository.LogIndicatorMetadataReviewAudit(reviewAudit);
            }
        }

        private bool HasTimestampChanged(IndicatorViewModel indicatorViewModel)
        {
            // Has user changed the timestamp?
            var initialTimestamp = indicatorViewModel.IndicatorMetadata.NextReviewTimestampInitialValue;
            var nextReviewTimestamp = indicatorViewModel.IndicatorMetadata.NextReviewTimestamp;

            if (nextReviewTimestamp.HasValue == false)
            {
                return false;
            }

            var hasTimestampChanged = initialTimestamp.HasValue == false ||
                                      initialTimestamp.Value != nextReviewTimestamp.Value;
            return hasTimestampChanged;
        }

        private string GetAuditMessage(string notes, DateTime? nextReviewTimestamp, bool hasTimestampChanged)
        {
            var sb = new StringBuilder();

            // Add notes
            if (notes != null)
            {
                sb.Append(notes);
                if (hasTimestampChanged)
                {
                    sb.Append(". ");
                }
            }

            // Add time stamp
            if (hasTimestampChanged)
            {
                // Only include timestamp message if the date has changed
                sb.Append("Next review date set to ");
                sb.Append(nextReviewTimestamp.Value.ToShortDateString());
            }

            return sb.ToString();
        }

        private static int GetGroupId(int domain, Profile profile)
        {
            var groupingMetadata = profile.GetSelectedGroupingMetadata(domain);
            if (groupingMetadata == null)
            {
                throw new FpmException("Grouping metadata not found, check the sequence values are not all zero");
            }

            var groupId = groupingMetadata.GroupId;
            return groupId;
        }

        private IEnumerable<SelectListItem> GetOrderedListOfDomains(SelectListItem defaultProfile)
        {
            var domains = new ProfileMembers();

            return CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);
        }

        private IEnumerable<SelectListItem> GetOrderedListOfProfiles()
        {
            var profileDetails = GetProfilesWithPermission();

            return CommonUtilities.GetOrderedListOfProfiles(profileDetails);
        }

        private IEnumerable<ProfileDetails> GetProfilesWithPermission()
        {
            var permissionIds = _reader.GetUserGroupPermissionsByUserId(_user.Id).Select(x => x.ProfileId);

            return _reader.GetProfiles()
                .OrderBy(x => x.Name)
                .Where(x => permissionIds.Contains(x.Id));
        }

        private int GetTimeSeries(Grouping grouping)
        {
            if (grouping.BaselineMonth == -1 && grouping.DataPointMonth == -1 && grouping.BaselineQuarter == -1 &&
                grouping.DataPointQuarter == -1)
            {
                return TimeSeries.Annual;
            }

            if (grouping.BaselineMonth == -1 && grouping.DataPointMonth == -1)
            {
                return TimeSeries.Quarter;
            }

            if (grouping.BaselineQuarter == -1 && grouping.DataPointQuarter == -1)
            {
                return TimeSeries.Month;
            }

            return TimeSeries.Annual;
        }
    }
}