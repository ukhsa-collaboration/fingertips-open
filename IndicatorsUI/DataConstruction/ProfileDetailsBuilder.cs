using System;
using System.Collections.Generic;
using System.Linq;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.DataConstruction
{
    public class ProfileDetailsBuilder
    {
        private static readonly char[] Divider = new[] { ',' };

        private ProfileReader profileReader;
        private ProfileDetails details;


        public ProfileDetailsBuilder(string profileKey)
        {
            CreateProfileDetails(profileKey);
        }

        public ProfileDetailsBuilder(int profileId)
        {
            CreateProfileDetails(profileId);
        }

        public static IList<string> ParseStringList(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new string[] { };
            }

            var list = s.Split(Divider, StringSplitOptions.RemoveEmptyEntries);

            return list
                .Select(x => x.Trim())
                .Where(x => string.IsNullOrEmpty(x) == false)
                .ToList();
        }

        private void AssignDomains()
        {
            var groupIds = profileReader.GetDomainIds(details.Id);
            details.Domains = profileReader.GetProfileDomains(groupIds);
        }

        private void CreateProfileDetails(string profileKey)
        {
            profileReader = ReaderFactory.GetProfileReader();
            details = profileReader.GetProfileDetails(profileKey);
            if (details != null)
            {
                details.ProfileUrlKey = profileKey;
                AssignDomains();

                // Parse string lists
                details.ExtraJavaScriptFiles = ParseStringList(details.ExtraJavaScriptFilesString);
                details.ExtraCssFiles = ParseStringList(details.ExtraCssFilesString);
            }
            else if (string.IsNullOrEmpty(profileKey) == false)
            {
                
                throw new FingertipsException("Profile could not be found: " + profileKey);
            }
        }

        private void CreateProfileDetails(int profileId)
        {
            profileReader = ReaderFactory.GetProfileReader();
            details = profileReader.GetProfileDetails(profileId);
            if (details != null)
            {
                details.Id = profileId;
                AssignDomains();

                // Parse string lists
                details.ExtraJavaScriptFiles = ParseStringList(details.ExtraJavaScriptFilesString);
                details.ExtraCssFiles = ParseStringList(details.ExtraCssFilesString);
            }
        }

        /// <summary>
        /// Creates the ProfileDetails instance and assigns the domain titles if any.
        /// </summary>
        /// <returns></returns>
        public ProfileDetails Build()
        {
            return details;
        }


    }
}
