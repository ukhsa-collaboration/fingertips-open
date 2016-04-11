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
            Delete
        }

        private static void AddPleaseSelectOption(List<SelectListItem> list)
        {
            list.Insert(0, new SelectListItem { Text = "Please select...", Value = "-1" });
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
                AddPleaseSelectOption(items);
            }
            return items;
        }


        public static Dictionary<int, string> GetAreaTypesLookUp()
        {
            return Reader.GetAllAreaTypes().ToDictionary(x => x.Id, x => x.ShortName);
        }

        public static IEnumerable<SelectListItem> GetListOfComparators(int? comparatorId = null)
        {
            var listOfComparators = new List<SelectListItem>
                {
                    new SelectListItem {
                        Text = "National & Subnational",
                        Value = ComparatorIds.NationalAndSubnational.ToString(),
                        Selected = comparatorId == ComparatorIds.NationalAndSubnational
                    },
                    new SelectListItem {
                        Text = "National Only",
                        Value = ComparatorIds.National.ToString(),
                        Selected = comparatorId == ComparatorIds.National
                    },
                    new SelectListItem {
                        Text = "Subnational Only",
                        Value = ComparatorIds.Subnational.ToString(),
                        Selected = comparatorId == ComparatorIds.Subnational
                    }
                };

            if (comparatorId == null)
            {
                // Comparator hasn't been set so need default option
                AddPleaseSelectOption(listOfComparators);
            }
            return listOfComparators;

        }

        public static IEnumerable<SelectListItem> GetListOfCiConfidenceLevels()
        {
            var listOfComparators = new List<SelectListItem>
                {
                    new SelectListItem { Text = "95", Value = "95" },
                    new SelectListItem { Text = "99.8", Value = "99.8" },
                    new SelectListItem { Text = "Not Applicable", Value = "-1" }
                };

            AddPleaseSelectOption(listOfComparators);
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
                AddPleaseSelectOption(listOfFrequencies);
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
                AddPleaseSelectOption(items);
            }

            return items;
        }

        public static IList<SelectListItem> GetListOfComparatorConfidences()
        {
            var items = Reader.GetAllComparatorConfidences()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                }).ToList();

            AddPleaseSelectOption(items);
            return items;
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfDomainsWithGroupId(ProfileMembers domains, SelectListItem defaultProfile, ProfileRepository profileRepository)
        {
            if (defaultProfile != null)
                domains.Profile = GetProfile(defaultProfile.Value, 0, -1, profileRepository);

            var listOfDomains = new SelectList(domains.Profile.GroupingMetadatas.OrderBy(g => g.Sequence), "GroupId", "GroupName");
            var selectedDomain = new SelectList(listOfDomains, "Value", "Key", listOfDomains.FirstOrDefault().Value).SelectedValue.ToString();
            return listOfDomains.Select(x => new SelectListItem { Selected = (x.Value == selectedDomain), Text = x.Text, Value = x.Value }); ;
        }

        public static IEnumerable<SelectListItem> GetOrderedListOfProfiles(IEnumerable<ProfileDetails> model)
        {
            return GetOrderedListOfProfilesWithSpecificProfileSelected(model, null);
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

        public static IEnumerable<SelectListItem> GetListOfPolarityTypes(PleaseSelectOption selectOption)
        {
            var polarities = Reader.GetAllPolarities();

            var listOfPolarities = GetSelectListOfPolarities(polarities);

            if (selectOption == PleaseSelectOption.Required)
            {
                AddPleaseSelectOption(listOfPolarities);
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

        public static ProfileDetails GetProfile(int profileId)
        {
            return Reader.GetProfileDetailsByProfileId(profileId);
        }

        public static Profile GetProfile(string urlKey, int selectedDomainNumber, int areaType, ProfileRepository profileRepository)
        {
            return new ProfileBuilder(profileRepository).Build(urlKey, selectedDomainNumber, areaType);
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
                AddPleaseSelectOption(listOfYearRanges);
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

        public static void CreateNewIndicatorTextValues(string selectedDomain, string userMTVChanges,
            IList<IndicatorMetadataTextProperty> properties, int nextIndicatorId, string userName, ProfileRepository profileRepository)
        {
            var items = new IndicatorMetadataTextParser().Parse(userMTVChanges);

            var allPropertiesToAdd = new List<IndicatorMetadataTextProperty>();

            foreach (var item in items)
            {
                IndicatorMetadataTextProperty property = properties.First(x => x.PropertyId == item.PropertyId);
                property.Text = item.Text;
                allPropertiesToAdd.Add(property);

                // Save to change log 
                profileRepository.LogPropertyChange(property.PropertyId, null, nextIndicatorId, Convert.ToInt32(selectedDomain), userName, DateTime.Now);
            }

            profileRepository.CreateIndicator(allPropertiesToAdd, nextIndicatorId);
        }

        public static bool IsReadOnlyMode()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["IsReadOnlyMode"]);
        }

        public static string GetTestSiteUrl(IndicatorGridModel model)
        {
            string url = GetTestSiteUrl(model.ProfileKey) + "#gid/" + model.SelectedGroupId;

            var areaTypeId = model.SelectedAreaTypeId;
            if (areaTypeId != AreaTypeIds.GpPractice)
            {
                url += "/ati/" + model.SelectedAreaTypeId;
            }

            return url;
        }

        public static string GetTestSiteUrl(string profileUrlKey)
        {
            return AppConfig.DefaultTestUrl + profileUrlKey;
        }

        public static string GetStatusUpdateMessage()
        {
            return ConfigurationManager.AppSettings["StatusUpdateMessage"];
        }

        public static IEnumerable<UserGroupPermissions> GetUserGroupPermissionsByUserId(int userId)
        {
            return Reader.GetUserGroupPermissionsByUserId(userId);
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




        public static void UpdateIndicatorTextValues(int indicatorId, int? groupId, string userMTVChanges,
            IList<IndicatorMetadataTextProperty> properties, DateTime timeOfChange, string userName, int? profileId, ProfileRepository profileRepository)
        {
            var items = new IndicatorMetadataTextParser().Parse(userMTVChanges);

            foreach (var item in items)
            {
                IndicatorMetadataTextProperty property = properties.First(x => x.PropertyId == item.PropertyId);

                // Get properties + existing values
                IndicatorText indicatorText = Reader.GetIndicatorTextValues(indicatorId,
                        new List<IndicatorMetadataTextProperty> { property },
                        profileId).First();

                // Figure out whether to update generic or specific
                int? profileIdForProperty = null;
                if ((item.IsOverridden || indicatorText.HasSpecificValue()) && groupId.HasValue)
                {
                    profileIdForProperty = profileId;
                }

                // Text that is being replaced
                string oldText;
                if (item.IsOverridden)
                {
                    // The user is overriding the generic value for the first time
                    oldText = null;
                }
                else if (indicatorText.HasSpecificValue())
                {
                    // The user is modifying an existing specific value
                    oldText = indicatorText.ValueSpecific;
                }
                else
                {
                    // User has changed the generic value
                    oldText = indicatorText.ValueGeneric;
                }

                // Save to change log 
                profileRepository.LogPropertyChange(property.PropertyId, oldText, indicatorId, profileIdForProperty, userName,
                                              timeOfChange);

                var indicatorAlreadyOverridden = profileRepository.DoesOverriddenIndicatorMetaDataRecordAlreadyExist(indicatorId, profileId);

                var text = item.Text;
                if (indicatorAlreadyOverridden && indicatorText.HasSpecificValue())
                {
                    if (text == indicatorText.ValueSpecific || text == indicatorText.ValueGeneric)
                    {
                        text = null;
                    }
                    profileRepository.UpdateProperty(property, text, indicatorId, profileIdForProperty);
                }
                else
                {
                    if (!indicatorAlreadyOverridden && item.IsOverridden)
                    {
                        profileRepository.CreateNewOverriddenIndicator(property, text, indicatorId, profileId);
                    }
                    else
                    {
                        if (text == indicatorText.ValueSpecific || text == indicatorText.ValueGeneric)
                        {
                            text = null;
                        }
                        profileRepository.UpdateProperty(property, text, indicatorId, profileIdForProperty);
                    }
                }
            }

            RemoveEmptyOverrides(indicatorId, groupId, properties, profileRepository);
        }

        public static void RemoveEmptyOverrides(int indicatorId, int? profileId, IList<IndicatorMetadataTextProperty> properties, ProfileRepository profileRepository)
        {
            //Clean-up - Remove any overridden Indicator Metadata records where all text property fields are null
            if (!Reader.GetIndicatorTextValues(indicatorId, properties, profileId).ToList().Any(x => x.HasSpecificValue()))
            {
                profileRepository.DeleteOverriddenMetaDataTextValues(indicatorId, (int)profileId);
            }
        }

        public static IEnumerable<Area> GetAreas(string searchText, int? areaTypeId)
        {
            return Reader.GetAreas(searchText, areaTypeId);
        }

        public static IEnumerable<AreaType> GetAllAreaTypes()
        {
            return Reader.GetAllAreaTypes();
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
            var exceptionServers = Reader.GetDistinctExceptionServers().ToList();
            exceptionServers.Add(ExceptionOptions.AllServers);

            return exceptionServers.Select(x => new SelectListItem { Text = x, Value = x, }).ToList();
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
    }
}