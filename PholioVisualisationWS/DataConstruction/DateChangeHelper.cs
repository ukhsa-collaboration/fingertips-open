using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;

namespace PholioVisualisation.DataConstruction
{
    public interface IDateChangeHelper
    {
        IndicatorDateChange GetIndicatorDateChange(TimePeriod timePeriod,
            IndicatorMetadata metadata, int newDataDeploymentCount);
    }

    public class DateChangeHelper : IDateChangeHelper
    {
        private readonly IMonthlyReleaseHelper _monthlyReleaseHelper;
        private readonly ICoreDataAuditRepository _coreDataAuditRepository;
        private readonly ICoreDataSetRepository _coreDataSetRepository;

        public DateChangeHelper(IMonthlyReleaseHelper monthlyReleaseHelper, ICoreDataAuditRepository coreDataAuditRepository,
            ICoreDataSetRepository coreDataSetRepository)
        {
            _monthlyReleaseHelper = monthlyReleaseHelper;
            _coreDataAuditRepository = coreDataAuditRepository;
            _coreDataSetRepository = coreDataSetRepository;
        }

        public IndicatorDateChange GetIndicatorDateChange(TimePeriod timePeriod,
            IndicatorMetadata metadata, int newDataDeploymentCount)
        {
            // Is this an indicator that should be highlighted as new
            if (metadata.ShouldNewDataBeHighlighted == false)
            {
                return IndicatorDateChange.GetNoChange();
            }

            // Is any there any audit information
            var coreDataUpload = GetLatestUploadAuditData(metadata.IndicatorId);
            if (coreDataUpload == null)
            {
                return IndicatorDateChange.GetNoChange();
            }

            // Is data from most recent time period
            var mostRecentTimePeriod = _coreDataSetRepository.GetLastestTimePeriodOfCoreData(metadata.IndicatorId, timePeriod.YearRange);
            if (mostRecentTimePeriod == null || mostRecentTimePeriod.ToString() != timePeriod.ToString())
            {
                // Data is not of the most recent available time period
                 return IndicatorDateChange.GetNoChange();
            }

            // Initialise the data changed to be false
            bool hasDataChanged = false;

            // If new data deployment count is set for the profile
            // then calculate the release date and date changed
            if (newDataDeploymentCount > 0)
            {
                // Calculate the release date
                DateTime releaseDate = _monthlyReleaseHelper.GetReleaseDate(newDataDeploymentCount);

                // Use override date is more recent than the release date
                if (metadata.LatestChangeTimestampOverride != null)
                {
                    var indicatorLatestChangeTimestampOverride = metadata.LatestChangeTimestampOverride.Value;
                    if (releaseDate < indicatorLatestChangeTimestampOverride)
                    {
                        releaseDate = indicatorLatestChangeTimestampOverride;
                    }
                }

                // Determine whether data changed recently
                hasDataChanged = releaseDate < coreDataUpload.DateCreated;
            }

            // Get the following release date after core data upload date created
            DateTime dateOfLastChange = _monthlyReleaseHelper.GetFollowingReleaseDate(coreDataUpload.DateCreated);

            // Return
            return new IndicatorDateChange
            {
                HasDataChangedRecently = hasDataChanged,
                DateOfLastChange = dateOfLastChange
            };
        }

        private CoreDataUploadAudit GetLatestUploadAuditData(int indicatorId)
        {
            return _coreDataAuditRepository.GetLatestUploadAuditData(indicatorId);
        }
    }
}
