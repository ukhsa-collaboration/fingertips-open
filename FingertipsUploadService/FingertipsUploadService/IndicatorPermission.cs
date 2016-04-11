using FingertipsUploadService.Helpers;
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Entities.Profile;
using FingertipsUploadService.ProfileData.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService
{
    public class IndicatorPermission
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        public bool Check(List<int> indicators, UploadJob job, UploadJobErrorRepository errorRepository)
        {
            bool hasPermissionCheckPassed;
            var permissionError = CheckIndicatorPermissionForCurrentUser(indicators, job.UserId);

            if (permissionError.Count > 0)
            {
                var error = job.JobType == UploadJobType.Simple
                    ? ErrorBuilder.GetSimplePermissionError(job.Guid, permissionError)
                    : ErrorBuilder.GetBatchPermissionError(job.Guid, permissionError);
                errorRepository.Log(error);
                hasPermissionCheckPassed = false;
            }
            else
            {
                hasPermissionCheckPassed = true;
            }

            return hasPermissionCheckPassed;
        }

        private List<string> CheckIndicatorPermissionForCurrentUser(List<int> indicators, int userId)
        {
            var permissions = new List<string>();
            // List of indicator ids in current batch
            var listOfIndicatorsInCurrentBatch = indicators;
            // Get list of profiles where current user has permission
            var user = new UserDetails(userId);
            var userProfiles = user.GetProfilesUserHasPermissionsTo().ToList();
            var userProfileIds = userProfiles.Select(x => x.Id).ToList();
            // Get the dictionary for indicators and owner profiles 
            var indicatorIds = _reader.GetIndicatorIdsByProfileIds(userProfileIds);

            var disallowedIndicators = new List<int>();

            foreach (int i in listOfIndicatorsInCurrentBatch)
            {
                if (!indicatorIds.ContainsKey(i))
                {
                    disallowedIndicators.Add(i);
                }
            }

            if (disallowedIndicators.Count > 0)
            {
                disallowedIndicators.Reverse();
                foreach (int indicatorId in disallowedIndicators)
                {
                    ProfileDetails disallowedProfile = _reader.GetOwnerProfilesByIndicatorIds(indicatorId);
                    string message = disallowedProfile != null
                        ? " is owned by " + disallowedProfile.Name
                        : " does not exist";
                    permissions.Add("Indicator " + indicatorId + message);
                }
            }

            return permissions;
        }
    }
}