using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Controllers
{
    public class IndicatorEditController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        private ProfileRepository _profileRepository;
        private LookUpsRepository _lookUpsRepository;

        private UserDetails _user;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _user = UserDetails.CurrentUser();
        }

        [HttpGet]
        [Route("profile/{urlKey}/area-type/{areatype}/domain/{selectedDomainNumber}/indicator/{indicatorId}/ageId/{ageId}/sexId/{sexId}")]
        public ActionResult IndicatorEdit(string urlKey, int areaType, int selectedDomainNumber,
            int indicatorId, int ageId, int sexId)
        {
            Profile profile = GetProfile(urlKey, selectedDomainNumber, areaType);

            // Get text properties of selected indicator
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();
            int groupId = profile.GetSelectedGroupingMetadata(selectedDomainNumber).GroupId;

            // Assemble model
            var model = new IndicatorEdit
            {
                SelectedIndicatorId = indicatorId,
                UrlKey = urlKey,
                Profile = profile,
                TextValues = _reader.GetIndicatorTextValues(indicatorId, properties, profile.Id).ToList()
            };

            // Prev/Next
            if (profile.IndicatorNames.Count > 0)
            {
                int index = -1;
                IList<GroupingPlusName> names = profile.IndicatorNames;
                for (int i = 0; i < names.Count(); i++)
                {
                    if (names[i].IndicatorId == indicatorId)
                    {
                        index = i;
                        break;
                    }
                }

                int prevIndex = index > 0 ? index - 1 : names.Count - 1;
                int nextIndex = index == names.Count - 1 ? 0 : index + 1;
                model.IndicatorIdNext = names[nextIndex].IndicatorId;
                model.IndicatorIdPrevious = names[prevIndex].IndicatorId;
            }

            //Get the indicatore meta data
            IndicatorMetadata indicatorMetaData = _reader.GetIndicatorMetadata(indicatorId);
            model.IndicatorMetadata = indicatorMetaData;

            IList<Grouping> groupList = _reader.GetGroupings(groupId);
            IEnumerable<Grouping> indicatorGroupData =
                groupList.Where(
                    g =>
                        g.IndicatorId == indicatorId &&
                        g.GroupId == groupId &&
                        g.AreaTypeId == areaType &&
                        g.AgeId == ageId &&
                        g.SexId == sexId);
            Grouping[] groupData = indicatorGroupData as Grouping[] ?? indicatorGroupData.ToArray();
            model.Grouping = groupData.FirstOrDefault();

            //Set the comparator Id if this is a multiple comparator grouping record
            if (groupData.Count() > 1)
            {
                //There are multiple comparator indicators
                if (model.Grouping != null)
                {
                    model.Grouping.ComparatorId = ComparatorIds.NationalAndSubnational;
                }
            }

            model.urlKey = urlKey;

            var listOfProfiles = CommonUtilities.GetOrderedListOfProfilesForCurrentUser(urlKey);

            ViewBag.listOfProfiles = listOfProfiles;

            var domains = new ProfileMembers();

            var defaultProfile = listOfProfiles.FirstOrDefault(x => x.Selected) ?? listOfProfiles.FirstOrDefault();
            if (defaultProfile != null)
            {
                defaultProfile.Selected = true;
            }

            ViewBag.listOfDomains = CommonUtilities.GetOrderedListOfDomainsWithGroupId(domains, defaultProfile, _profileRepository);

            IEnumerable<UserGroupPermissions> userPermissions =
                CommonUtilities.GetUserGroupPermissionsByUserId(_reader.GetUserByUserName(_user.Name).Id);
            model.UserGroupPermissions =
                userPermissions.FirstOrDefault(x => x.ProfileId == _reader.GetProfileDetails(model.UrlKey).Id);

            if (HttpContext.Request.UrlReferrer != null) model.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            ViewBag.SexId = new SelectList(_lookUpsRepository.GetSexes(), "SexID", "Description");

            ViewBag.AgeId = new SelectList(_lookUpsRepository.GetAges(), "AgeID", "Description");

            ViewBag.YearTypeId = new SelectList(_lookUpsRepository.GetYearTypes(), "Id", "Label");

            ViewBag.ValueTypeId = new SelectList(_lookUpsRepository.GetIndicatorValueTypes(), "Id", "Label");

            ViewBag.CIMethodId = new SelectList(_lookUpsRepository.GetConfidenceIntervalMethods(), "Id", "Name");

            var unitList = new SelectList(_lookUpsRepository.GetUnits(), "Id", "Label");
            ViewBag.UnitId = unitList;

            ViewBag.DenominatorTypeId = new SelectList(_lookUpsRepository.GetDenominatorTypes(), "Id", "Name");

            return View("IndicatorEdit", model);
        }

        [HttpPost]
        [Route("profile/{urlKey}/area-type/{areatype}/domain/{selectedDomainNumber}/indicator/{indicatorId}")]
        public ActionResult IndicatorEditSave(int? decimalPlaces, int? targetId, bool alwaysShowSpineChart,
            int disclosureControlId, string urlKey, int areaType, int selectedDomainNumber,
            int indicatorId, int valueTypeId, int ciMethodId, string ciComparatorConfidence, int polarityId,
            int unitId, int denominatorTypeId, int yearTypeId, int areaTypeId, int sexId, int ageId, int comparatorId,
            int comparatorMethodId, double comparatorConfidence, int yearRange, int selectedFrequency, int baselineYear,
            int datapointYear, string returnUrl, int indicatorSequence, int currentAgeId, int currentSexId,
            int currentAreaTypeId, string userMTVChanges, DateTime? latestChangeTimestamp, int startQuarterRange = -1, int endQuarterRange = -1,
            int startMonthRange = -1, int endMonthRange = -1)
        {
            Profile profile = GetProfile(urlKey, selectedDomainNumber, areaType);

            int groupId = profile.GetSelectedGroupingMetadata(selectedDomainNumber).GroupId;

            //            var latestChangeTimestampF = DateTime.Now;
            SaveMetadataChanges(indicatorId, groupId, valueTypeId, ciMethodId, ciComparatorConfidence, polarityId,
                unitId,
                denominatorTypeId, yearTypeId, areaTypeId, sexId, ageId, comparatorId, comparatorMethodId,
                comparatorConfidence,
                yearRange, selectedFrequency, baselineYear, datapointYear, startQuarterRange, endQuarterRange,
                startMonthRange,
                endMonthRange, indicatorSequence, currentAgeId, currentSexId, currentAreaTypeId, decimalPlaces, targetId,
                alwaysShowSpineChart, profile.Id, disclosureControlId, latestChangeTimestamp);

            return Redirect(returnUrl);
        }

        private void SaveMetadataChanges(int indicatorId, int? groupId, int valueTypeId, int ciMethodId,
            string ciComparatorConfidence, int polarityId, int unitId, int denominatorTypeId, int yearTypeId,
            int areaTypeId, int sexId, int ageId, int comparatorId, int comparatorMethodId, double comparatorConfidence,
            int yearRange, int selectedFrequency, int baselineYear, int datapointYear, int baselineQuarter,
            int dataPointQuarter, int baselineMonth, int dataPointMonth, int indicatorSequence, int currentAgeId,
            int currentSexId, int currentAreaTypeId, int? decimalPlaces, int? targetId, bool alwaysShowSpineChart,
            int profileId, int disclosureControlId, DateTime? somedate)
        {
            switch (selectedFrequency)
            {
                case Frequencies.Annual:
                    baselineMonth = -1;
                    dataPointMonth = -1;
                    baselineQuarter = -1;
                    dataPointQuarter = -1;
                    break;
                case Frequencies.Quarterly:
                    baselineMonth = -1;
                    dataPointMonth = -1;
                    break;
                case Frequencies.Monthly:
                    baselineQuarter = -1;
                    dataPointQuarter = -1;
                    break;
            }

            var indicatorMetadataTextChanges = SaveIndicatorMetadataTextChanges(indicatorId, profileId);

            _profileRepository.UpdateGroupingAndMetadata(Convert.ToInt32(groupId), indicatorId, areaTypeId, sexId, ageId,
                comparatorId, comparatorMethodId, ciComparatorConfidence, comparatorConfidence, yearTypeId, yearRange, valueTypeId,
                ciMethodId, polarityId, unitId, denominatorTypeId, baselineYear, datapointYear, baselineQuarter,
                dataPointQuarter, baselineMonth, dataPointMonth, indicatorSequence, currentAgeId, currentSexId,
                currentAreaTypeId, indicatorMetadataTextChanges, _user.Name, CommonUtilities.AuditType.Change.ToString(),
                decimalPlaces, targetId, alwaysShowSpineChart, disclosureControlId, somedate);
        }

        private string[] SaveIndicatorMetadataTextChanges(int indicatorId, int profileId)
        {
            IList<IndicatorMetadataTextProperty> properties = _reader.GetIndicatorMetadataTextProperties();

            string metadataTextChanges = Request.Params["userMTVChanges"];

            string[] allChanges = Array.ConvertAll(Request.Params["userOtherChanges"].Split(Convert.ToChar("¬")), s => s);

            // Update the Indicator Meta Data Text Values
            if (string.IsNullOrWhiteSpace(metadataTextChanges) == false)
            {
                var textChangesByUser = new IndicatorMetadataTextParser().Parse(metadataTextChanges);

                var ownerProfileId = _reader.GetIndicatorMetadata(indicatorId).OwnerProfileId;
                var isOwnerProfileBeingEdited = profileId == ownerProfileId;
                CheckUserAllowedToMakeChange(isOwnerProfileBeingEdited, ownerProfileId);

                new IndicatorMetadataTextChanger(new ProfileRepository(NHibernateSessionFactory.GetSession()), _reader)
                    .UpdateIndicatorTextValues(indicatorId, textChangesByUser, properties, _user.Name, profileId,
                        isOwnerProfileBeingEdited);
            }
            return allChanges;
        }

        private void CheckUserAllowedToMakeChange(bool isOwnerProfileBeingEdited, int ownerProfileId)
        {
            if (isOwnerProfileBeingEdited && _user.HasWritePermissionsToProfile(ownerProfileId) == false)
            {
                throw new FpmException(string.Format("User {0} should not be able to edit the owner profile {1}",
                    _user.Name, ownerProfileId));
            }
        }

        private Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType)
        {
            return new ProfileBuilder(_profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profileRepository = new ProfileRepository(NHibernateSessionFactory.GetSession());
            _lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();
            _lookUpsRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }
    }
}