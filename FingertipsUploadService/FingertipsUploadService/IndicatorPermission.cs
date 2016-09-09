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
        private List<string> noneExistingIndicators = new List<string>();
        private List<string> indicatorsWithoutPermission = new List<string>();



        public bool Check(List<int> indicators, UploadJob job, UploadJobErrorRepository errorRepository)
        {

            // Check permission
            CheckIndicatorPermissionForCurrentUser(indicators, job.UserId);

            // Check if all indicator exist
            if (noneExistingIndicators.Count > 0)
            {
                var error = job.JobType == UploadJobType.Simple
                    ? ErrorBuilder.GetSimplePermissionError(job.Guid, noneExistingIndicators, false)
                    : ErrorBuilder.GetBatchPermissionError(job.Guid, noneExistingIndicators, false);
                errorRepository.Log(error);
                return false;
            }

            // Check permissions
            if (indicatorsWithoutPermission.Count > 0)
            {
                var error = job.JobType == UploadJobType.Simple
                    ? ErrorBuilder.GetSimplePermissionError(job.Guid, indicatorsWithoutPermission, true)
                    : ErrorBuilder.GetBatchPermissionError(job.Guid, indicatorsWithoutPermission, true);
                errorRepository.Log(error);
                return false;
            }

            return true;
        }

        private void CheckIndicatorPermissionForCurrentUser(List<int> indicators, int userId)
        {
            //            var permissions = new List<string>();
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

                    if (disallowedProfile != null)
                    {
                        var permissionMessage = "Indicator " + indicatorId + " is owned by " + disallowedProfile.Name;
                        indicatorsWithoutPermission.Add(permissionMessage);
                    }
                    else
                    {
                        var noneExistingMessage = "" + indicatorId;
                        noneExistingIndicators.Add(noneExistingMessage);
                    }
                }
            }
        }
    }
}