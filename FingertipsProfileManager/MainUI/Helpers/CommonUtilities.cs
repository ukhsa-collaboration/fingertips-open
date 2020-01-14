using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IndicatorAudit = Fpm.ProfileData.Entities.Profile.IndicatorAudit;

namespace Fpm.MainUI.Helpers
{
    public static class CommonUtilities
    {
        private static readonly ProfilesReader Reader = ReaderFactory.GetProfilesReader();

        public const int QuartersInOneYear = 4;

        public enum AuditType
        {
            Copy,
            Move,
            Change,
            Delete,
            Remove
        }

        private static void AddPleaseSelectOption(List<SelectListItem> list, int value)
        {
            list.Insert(0, new SelectListItem { Text = "Please select...", Value = value.ToString() });
        }

        public static List<SelectListItem> GetListOfAreaTypes(PleaseSelectOption pleaseSelectOption,
            int selectedAreaTypeId = -1)
        {
            return GetAreaTypeItems(pleaseSelectOption, selectedAreaTypeId, Reader.GetAllAreaTypes());
        }

        public static List<SelectListItem> GetListOfSupportedAreaTypes(PleaseSelectOption pleaseSelectOption,
            int selectedAreaTypeId = -1)
        {
            return GetAreaTypeItems(pleaseSelectOption, selectedAreaTypeId, Reader.GetSupportedAreaTypes());
        }



        private static List<SelectListItem> GetAreaTypeItems(PleaseSelectOption pleaseSelectOption, int selectedAreaTypeId,
            IList<AreaType> areaTypes)
        {
            var items = new AreaTypeSelectListBuilder(areaTypes, selectedAreaTypeId).SelectListItems;
            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(items, -1);
            }
            return items;
        }


        public static Dictionary<int, string> GetAreaTypesLookUp()
        {
            return Reader.GetAllAreaTypes().ToDictionary(x => x.Id, x => x.ShortName);
        }

        public static IEnumerable<SelectListItem> GetListOfCiConfidenceLevels()
        {
            var listOfComparators = new List<SelectListItem>
                {
                    new SelectListItem { Text = "95", Value = "95" },
                    new SelectListItem { Text = "99.8", Value = "99.8" },
                    new SelectListItem { Text = "Not Applicable", Value = "-1" }
                };

            AddPleaseSelectOption(listOfComparators, -99);
            return listOfComparators;

        }

        public static IEnumerable<SelectListItem> GetFrequencies(PleaseSelectOption selectionOption)
        {
            var listOfFrequencies = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Annual", Value = Frequencies.Annual.ToString() },
                    new SelectListItem { Text = "Monthly", Value = Frequencies.Monthly.ToString()},
                    new SelectListItem { Text = "Quarterly", Value = Frequencies.Quarterly.ToString() }
                };

            if (selectionOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(listOfFrequencies, -1);
            }

            return listOfFrequencies;

        }

        public static IEnumerable<SelectListItem> GetListOfComparatorMethods(PleaseSelectOption selectOption)
        {
            var items = Reader.GetAllComparatorMethods()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();

            if (selectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(items, -1);
            }

            return items;
        }

        public static IList<SelectListItem> GetListOfComparatorConfidences()
        {
            var confidences = Reader.GetAllComparatorConfidences();

            var items = new List<SelectListItem>();

            foreach (var confidence in confidences)
            {
                var item = new SelectListItem
                {
                    Value = confidence.ToString(),
                    Text = string.Format("{0:0.0}", confidence)
                };

                items.Add(item);
            }

            AddPleaseSelectOption(items, -1);
            return items;
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfDomainsWithGroupId(ProfileMembers domains,
            SelectListItem defaultProfile, IProfileRepository profileRepository, int? selectedGroupId = null)
        {
            if (defaultProfile != null)
            {

                domains.Profile = GetProfile(defaultProfile.Value, 0, -1, profileRepository);

                var groupingMetadataList = domains.Profile.GroupingMetadatas.OrderBy(g => g.Sequence);
                var listOfDomains = new SelectList(groupingMetadataList, "GroupId", "GroupName");
                var groupId = selectedGroupId.HasValue 
                    ? selectedGroupId.Value.ToString()
                    : groupingMetadataList.First().GroupId.ToString();

                return listOfDomains.Select(x => new SelectListItem
                {
                    Selected = x.Value == groupId,
                    Text = x.Text,
                    Value = x.Value
                });
            }

            return new List<SelectListItem>();
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfProfiles(IEnumerable<ProfileDetails> profileDetailsList)
        {
            return GetOrderedListOfProfilesWithSpecificProfileSelected(profileDetailsList, null);
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfProfilesWithSpecificProfileSelected(
            IEnumerable<ProfileDetails> model, string profileName)
        {
            var listOfProfiles = new SelectList(model.OrderBy(x => x.Name), "UrlKey", "Name", profileName);
            return listOfProfiles;
        }


        public static IEnumerable<SelectListItem> GetListOfTargetPolarityTypes()
        {
            // Target polarities are limited to those below
            var targetPolarityIds = new List<int>
            {
                PolarityIds.RagHighIsGood,
                PolarityIds.RagLowIsGood,
                PolarityIds.UseBlues
            };

            var polarities = Reader.GetAllPolarities()
                .Where(x => targetPolarityIds.Contains(x.Id))
                .ToList();

            return GetSelectListOfPolarities(polarities);
        }

        public static IEnumerable<SelectListItem> GetListOfPolarityTypes(PleaseSelectOption selectOption, int comparatorMethodId = ComparatorMethodIds.NoComparison)
        {
            var polarities = Reader.GetAllPolarities();

            var listOfPolarities = GetSelectListOfPolarities(polarities);

            if (comparatorMethodId == ComparatorMethodIds.Quintiles)
            {
                foreach (var polarity in listOfPolarities)
                {
                    switch (Convert.ToInt32(polarity.Value))
                    {
                        case PolarityIds.NotApplicable:
                            polarity.Text = "No judgement";
                            break;
                        case PolarityIds.RagHighIsGood:
                            polarity.Text = "High is good";
                            break;
                        case PolarityIds.RagLowIsGood:
                            polarity.Text = "Low is good";
                            break;
                    }
                }
            }

            if (selectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(listOfPolarities, -99);
            }

            return listOfPolarities;
        }

        private static List<SelectListItem> GetSelectListOfPolarities(IList<Polarity> polarities)
        {
            var listOfPolarities = polarities.Select(polarity => new SelectListItem
            {
                Value = polarity.Id.ToString(),
                Text = polarity.Name
            }).ToList();
            return listOfPolarities;
        }


        public static IEnumerable<SelectListItem> GetListOfTargets()
        {
            var selectListItems = new List<SelectListItem>();
            var targets = Reader.GetAllTargets();

            foreach (var targetConfig in targets)
            {
                var listItem = new SelectListItem
                {
                    Value = targetConfig.Id.ToString(),
                    Text = targetConfig.Description
                };
                selectListItems.Add(listItem);
            }

            selectListItems.Insert(0, new SelectListItem
            {
                Value = string.Empty/*will be interpreted as null on postback convertion to int?*/,
                Text = "None"
            });

            return selectListItems;
        }

        public static IEnumerable<SelectListItem> GetListOfDisclosureControl()
        {
            var selectedListItems = new List<SelectListItem>();
            var disclosures = Reader.GetAllDisclosureControl();
            foreach (var disclosure in disclosures)
            {
                var listItem = new SelectListItem
                {
                    Value = disclosure.Id.ToString(),
                    Text = disclosure.Name
                };
                selectedListItems.Add(listItem);
            }

            return selectedListItems;
        }

        public static ProfileDetails GetProfile(int profileId)
        {
            return Reader.GetProfileDetailsByProfileId(profileId);
        }

        public static Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType, IProfileRepository profileRepository)
        {
            return new ProfileBuilder(Reader, profileRepository).Build(urlKey, selectedDomainNumber, areaType);
        }

        public static IEnumerable<SelectListItem> GetListOfYearRanges(PleaseSelectOption selectionOption)
        {
            var listOfYearRanges = new List<SelectListItem>
                {
                    new SelectListItem { Text = "1 Year", Value = "1" },
                    new SelectListItem { Text = "2 Years", Value = "2" },
                    new SelectListItem { Text = "3 Years", Value = "3" },
                    new SelectListItem { Text = "4 Years", Value = "4" },
                    new SelectListItem { Text = "5 Years", Value = "5" },
                    new SelectListItem { Text = "6 Years", Value = "6" },
                    new SelectListItem { Text = "7 Years", Value = "7" },
                    new SelectListItem { Text = "8 Years", Value = "8" },
                    new SelectListItem { Text = "9 Years", Value = "9" },
                    new SelectListItem { Text = "10 Years", Value = "10" }
                };

            if (selectionOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(listOfYearRanges, -1);
            }

            return listOfYearRanges;
        }

        public static IEnumerable<SelectListItem> GetQuarters()
        {
            return GetNumericSelectList(QuartersInOneYear);
        }

        public static IEnumerable<SelectListItem> GetQuarters(int yearTypeId, int yearRange)
        {
            int quarters = QuartersInOneYear;

            if (yearTypeId == YearTypeIds.FinancialMultiYearCumulativeQuarter)
            {
                quarters = yearRange * QuartersInOneYear;
            }
            return GetNumericSelectList(quarters);
        }

        public static IEnumerable<SelectListItem> GetMonths()
        {
            return GetNumericSelectList(12);
        }

        private static List<SelectListItem> GetNumericSelectList(int upper)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 1; i <= upper; i++)
            {
                var s = i.ToString();
                items.Add(new SelectListItem
                {
                    Value = s,
                    Text = s
                });
            }
            return items;
        }

        public static bool IsReadOnlyMode()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["IsReadOnlyMode"]);
        }

        public static string GetTestSiteUrl(IndicatorGridModel model)
        {
            return GetTestSiteUrl(model.ProfileKey) + "#gid/" + model.SelectedGroupId + "/ati/" + model.SelectedAreaTypeId;
        }

        public static string GetTestSiteUrl(string profileUrlKey)
        {
            return AppConfig.DefaultTestUrl + profileUrlKey;
        }

        public static string GetTestSiteUrl()
        {
            return AppConfig.DefaultTestUrl;
        }

        public static string GetStatusUpdateMessage()
        {
            return ConfigurationManager.AppSettings["StatusUpdateMessage"];
        }

        public static IEnumerable<UserGroupPermissions> GetUserGroupPermissionsByUserId(int userId)
        {
            return Reader.GetUserGroupPermissionsByUserId(userId);
        }

        public static IEnumerable<UserGroupPermissions> GetContactUsersByProfile(int profileId)
        {
            var contactUserIdsOfString = Reader.GetProfileDetailsByProfileId(profileId).ContactUserIds.Split(',').ToArray();
            var contactUserIds = Array.ConvertAll(contactUserIdsOfString, Int32.Parse);
            return Reader.GetProfileUsers(profileId).Where(user => contactUserIds.Contains(user.UserId)); 
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfProfilesForCurrentUser(BaseDataModel model)
        {
            model.Profiles = UserDetails.CurrentUser().GetProfilesUserHasPermissionsTo();
            var currentProfile = model.UrlKey;
            var listOfProfiles = GetOrderedListOfProfilesWithSpecificProfileSelected(model.Profiles, currentProfile);
            return listOfProfiles;
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfProfilesForCurrentUser(string urlKey)
        {
            var profiles = UserDetails.CurrentUser().GetProfilesUserHasPermissionsTo();

            return GetOrderedListOfProfilesWithSpecificProfileSelected(profiles, urlKey);
        }

        public static IEnumerable<Area> GetAreas(string searchText, int? areaTypeId)
        {
            return Reader.GetAreas(searchText, areaTypeId);
        }

        public static IEnumerable<AreaType> GetAllAreaTypes()
        {
            return Reader.GetAllAreaTypes();
        }

        public static IEnumerable<FpmUser> GetAllFpmUsers()
        {
            IUserRepository userRepository = new UserRepository(NHibernateSessionFactory.GetSession());
            return userRepository.GetAllFpmUsers();
        }

        public static IEnumerable<IndicatorAudit> GetIndicatorAudit(List<int> indicatorList)
        {
            return Reader.GetIndicatorAudit(indicatorList);
        }

        public static IEnumerable<Grouping> GetGroupingsByIndicatorIds(List<int> indicatorList)
        {
            return Reader.GetGroupingByIndicatorId(indicatorList);
        }

        public static IList<GroupingMetadata> GetGroupingMetadata(List<int> groupingList)
        {
            return Reader.GetGroupingMetadataList(groupingList);
        }

        public static IList<ProfileDetails> GetProfiles()
        {
            return Reader.GetProfiles();
        }

        public static string GetIndicatorUrl(int indicatorId, int ageId, int sexId, string urlKey, int selectedAreaType, int selectedDomain)
        {
            return "/profile/" + urlKey + "/area-type/" + selectedAreaType + "/domain/" + selectedDomain +
                "/indicator/" + indicatorId + "/ageId/" + ageId + "/sexId/" + sexId;
        }

        public static string GetIndicatorUrl(string urlReferrerScheme, string urlReferrerAuthority, string urlReferrerAbsolutePath, string profileUrlKey, int domainSequence, int areaTypeId)
        {
            return string.Format("{0}://{1}{2}?ProfileKey={3}&DomainSequence={4}&selectedAreaTypeId={5}", urlReferrerScheme, urlReferrerAuthority,
                urlReferrerAbsolutePath, profileUrlKey, domainSequence, areaTypeId);
        }

        public static IEnumerable<SelectListItem> GetListOfDecimalPlaces()
        {
            var listOfDecimalPlaces = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Automatic",
                        Value = string.Empty/*will be interpreted as null on postback convertion to int?*/},
                    new SelectListItem { Text = "0", Value = "0" },
                    new SelectListItem { Text = "1", Value = "1" },
                    new SelectListItem { Text = "2", Value = "2" },
                    new SelectListItem { Text = "3", Value = "3" },
                };

            return listOfDecimalPlaces;

        }

        public static IEnumerable<SelectListItem> GetExceptionDays()
        {
            var listOfQuarters = new List<SelectListItem>
                {
                    new SelectListItem { Text = "0", Value = "0"},
                    new SelectListItem { Text = "1", Value = "1"},
                    new SelectListItem { Text = "2", Value = "2" },
                    new SelectListItem { Text = "3", Value = "3" },
                    new SelectListItem { Text = "4", Value = "4" },
                    new SelectListItem { Text = "5", Value = "5" },
                    new SelectListItem { Text = "10", Value = "10" }
                };

            return listOfQuarters;
        }

        public static IEnumerable<SelectListItem> GetDistinctExceptionServers()
        {
            var exceptionServers = new List<string>
            {
                ExceptionOptions.AllServers
            };

            IExceptionsRepository exceptionsRepository = new ExceptionsRepository(NHibernateSessionFactory.GetSession());
            var exceptionServersFromDb = exceptionsRepository.GetDistinctExceptionServers().ToList();

            foreach (var exceptionServer in exceptionServersFromDb)
            {
                exceptionServers.Add(exceptionServer);
            }

            return exceptionServers.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
        }

        public static IEnumerable<SelectListItem> GetLiveExceptionServers()
        {
            var liveExceptionServers = new List<string>
            {
                ExceptionOptions.AllServers,
                ServerNames.Live1.ToUpper(),
                ServerNames.Live2.ToUpper()
            };

            return liveExceptionServers.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
        }

        public static HtmlString GetContentItem(string contentKey, int profileId)
        {
            var contentItem = Reader.GetContentItem(contentKey, profileId);

            if (contentItem == null)
            {
                return new HtmlString(string.Empty);
            }

            return new HtmlString(HttpUtility.HtmlDecode(contentItem.Content));
        }

        public static bool IsDomainListAvailable(IEnumerable<SelectListItem> domainList)
        {
            return (domainList != null && domainList.Count() > 0);
        }

        public static List<SelectListItem> GetListOfAllAges(PleaseSelectOption pleaseSelectOption, int selectedAgeId = -1)
        {
            return GetAllAgeItems(pleaseSelectOption, selectedAgeId, Reader.GetAllAges());
        }

        private static List<SelectListItem> GetAllAgeItems(PleaseSelectOption pleaseSelectOption, int selectedAgeId, IList<Age> ages)
        {
            var items = new AgeSelectListBuilder(ages, selectedAgeId).SelectListItems;
            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(items, -1);
            }

            return items;
        }

        public static List<SelectListItem> GetListOfAllSexes(PleaseSelectOption pleaseSelectOption, int selectedSexId = -1)
        {
            return GetAllSexItems(pleaseSelectOption, selectedSexId, Reader.GetAllSexes());
        }

        private static List<SelectListItem> GetAllSexItems(PleaseSelectOption pleaseSelectOption, int selectedSexId, IList<Sex> sexes)
        {
            var items = new SexSelectListBuilder(sexes, selectedSexId).SelectListItems;
            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(items, -99);
            }

            return items;
        }

        public static IEnumerable<SelectListItem> GetIndicatorValueTypes(PleaseSelectOption pleaseSelectOption)
        {
            var selectedListItems = new List<SelectListItem>();
            var lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());
            var indicatorValueTypes = lookUpsRepository.GetIndicatorValueTypes();

            foreach (var indicatorValueType in indicatorValueTypes)
            {
                var listItem = new SelectListItem
                {
                    Value = indicatorValueType.Id.ToString(),
                    Text = indicatorValueType.Label
                };

                selectedListItems.Add(listItem);
            }

            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(selectedListItems, -1);
            }

            return selectedListItems;
        }

        public static IEnumerable<SelectListItem> GetConfidenceIntervalMethods(PleaseSelectOption pleaseSelectOption)
        {
            var selectedListItems = new List<SelectListItem>();
            var lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());
            var confidenceIntervalMethods = lookUpsRepository.GetConfidenceIntervalMethods();

            foreach (var confidenceIntervalMethod in confidenceIntervalMethods)
            {
                var listItem = new SelectListItem
                {
                    Value = confidenceIntervalMethod.Id.ToString(),
                    Text = confidenceIntervalMethod.Name
                };

                selectedListItems.Add(listItem);
            }

            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(selectedListItems, -1);
            }

            return selectedListItems;
        }

        public static IEnumerable<SelectListItem> GetUnits(PleaseSelectOption pleaseSelectOption)
        {
            var selectedListItems = new List<SelectListItem>();
            var lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());
            var units = lookUpsRepository.GetUnits();

            foreach (var unit in units)
            {
                var listItem = new SelectListItem
                {
                    Value = unit.Id.ToString(),
                    Text = unit.Label
                };

                selectedListItems.Add(listItem);
            }

            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(selectedListItems, -1);
            }

            return selectedListItems;
        }

        public static IEnumerable<SelectListItem> GetDenominatorTypes(PleaseSelectOption pleaseSelectOption)
        {
            var selectedListItems = new List<SelectListItem>();
            var lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());
            var denominatorTypes = lookUpsRepository.GetDenominatorTypes();

            foreach (var denominatorType in denominatorTypes)
            {
                var listItem = new SelectListItem
                {
                    Value = denominatorType.Id.ToString(),
                    Text = denominatorType.Name
                };

                selectedListItems.Add(listItem);
            }

            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(selectedListItems, -1);
            }

            return selectedListItems;
        }

        public static IEnumerable<SelectListItem> GetYearTypes(PleaseSelectOption pleaseSelectOption)
        {
            var selectedListItems = new List<SelectListItem>();
            var lookUpsRepository = new LookUpsRepository(NHibernateSessionFactory.GetSession());
            var yearTypes = lookUpsRepository.GetYearTypes();

            foreach (var yearType in yearTypes)
            {
                var listItem = new SelectListItem
                {
                    Value = yearType.Id.ToString(),
                    Text = yearType.Label
                };

                selectedListItems.Add(listItem);
            }

            if (pleaseSelectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(selectedListItems, -1);
            }

            return selectedListItems;
        }

        public static List<GroupingPlusName> GetSpecifiedIndicatorNames(List<IndicatorSpecifier> indicatorSpecifiers, IList<GroupingPlusName> allIndicators)
        {
            List<GroupingPlusName> indicatorList = new List<GroupingPlusName>();
            foreach (var indicatorSpecifier in indicatorSpecifiers)
            {
                var indicatorId = indicatorSpecifier.IndicatorId;
                var sexId = indicatorSpecifier.SexId;
                var ageId = indicatorSpecifier.AgeId;

                var selectedIndicator =
                    allIndicators
                        .Where(x => x.IndicatorId == indicatorId)
                        .Where(x => x.SexId == sexId)
                        .Where(x => x.AgeId == ageId)
                        .ToList()
                        .FirstOrDefault();

                indicatorList.Add(selectedIndicator);
            }
            return indicatorList;
        }

        public static string GetIndicatorNameWithSexAndAgeLabel(GroupingPlusName indicator)
        {
            return string.Format("{0} ({1}) ({2})", indicator.IndicatorName, indicator.Sex, indicator.Age);
        }

        public static IEnumerable<Grouping> GetGroupingsForIndicatorInProfile(int profileId, int indicatorId)
        {
            var groupingsForAllProfiles = Reader.GetGroupingByIndicatorId(new List<int> { indicatorId });

            var profileGroupIds = Reader.GetGroupingIds(profileId);

            var groupingsForCurrentProfile = groupingsForAllProfiles
                .Where(x => profileGroupIds.Contains(x.GroupId));

            return groupingsForCurrentProfile;
        }

        public static List<Grouping> GetDistinctGroupingsByGroupIdAndAreaTypeId(IEnumerable<Grouping> groupings)
        {
            var distinctGroupingsForProfile = new List<Grouping>();
            foreach (Grouping grouping in groupings)
            {
                var isAlreadyFound = false;
                foreach (var distinctGrouping in distinctGroupingsForProfile)
                {
                    if (grouping.GroupId == distinctGrouping.GroupId &&
                        grouping.AreaTypeId == distinctGrouping.AreaTypeId)
                    {
                        isAlreadyFound = true;
                        break;
                    }
                }

                if (isAlreadyFound == false)
                {
                    distinctGroupingsForProfile.Add(grouping);
                }
            }
            return distinctGroupingsForProfile;
        }

        public static UserGroupPermissions GetProfileUserPermissions(IEnumerable<UserGroupPermissions> userPermissions, string urlKey)
        {
            var profileId = Reader.GetProfileIdFromUrlKey(urlKey);

            return userPermissions.FirstOrDefault(
                x => x.ProfileId == profileId);
        }
    }
}