using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;

namespace PholioVisualisation.DataConstruction
{
    public interface IDateChangeHelper
    {
        IndicatorDateChange GetIndicatorDateChange(IndicatorMetadata metadata, int newDataDeploymentCount,
            int areaTypeId);
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

        public IndicatorDateChange GetIndicatorDateChange(IndicatorMetadata metadata, int newDataDeploymentCount,
            int areaTypeId)
        {
            // Is this an indicator that should be highlighted as new
            if (metadata.ShouldNewDataBeHighlighted == false)
            {
                return IndicatorDateChange.GetNoChange();
            }

            // Is there any change log recorded for this indicator and area type combination
            var coreDataSetChangeLog = _coreDataSetRepository.GetCoreDataSetChangeLog(metadata.IndicatorId, areaTypeId);
            if (coreDataSetChangeLog == null)
            {
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
                hasDataChanged = releaseDate < coreDataSetChangeLog.DateUpdated;
            }

            // Get the following release date after core data upload date created
            DateTime dateOfLastChange = _monthlyReleaseHelper.GetFollowingReleaseDate(coreDataSetChangeLog.DateUpdated);

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
