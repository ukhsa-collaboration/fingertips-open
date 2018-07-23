using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;

namespace PholioVisualisation.DataConstruction
{
    public class DateChangeHelper
    {
        private IMonthlyReleaseHelper _monthlyReleaseHelper;
        private ICoreDataAuditRepository _coreDataAuditRepository;

        public DateChangeHelper(IMonthlyReleaseHelper monthlyReleaseHelper, ICoreDataAuditRepository coreDataAuditRepository)
        {
            _monthlyReleaseHelper = monthlyReleaseHelper;
            _coreDataAuditRepository = coreDataAuditRepository;
        }

        public IndicatorDateChange AssignDateChange(IndicatorMetadata metadata, int newDataTimeSpanInDays)
        {
            // Is any there any audit information
            if (metadata.ShouldNewDataBeHighlighted == false)
            {
                return IndicatorDateChange.GetNoChange();
            }

            // Is any there any audit information
            var coreDataUpload = _coreDataAuditRepository.GetLatestUploadAuditData(metadata.IndicatorId);
            if (coreDataUpload == null)
            {
                return IndicatorDateChange.GetNoChange();
            }

            var dateOfLastUpload = coreDataUpload.DateCreated;

            // Get the release date that follows the last upload
            DateTime releaseDate = _monthlyReleaseHelper.GetFollowingReleaseDate(dateOfLastUpload);

            if (metadata.LatestChangeTimestampOverride != null)
            {
                // Use override date is more recent than the release date
                var indicatorLatestChangeTimestampOverride = metadata.LatestChangeTimestampOverride.Value;
                if (releaseDate < indicatorLatestChangeTimestampOverride)
                {
                    releaseDate = indicatorLatestChangeTimestampOverride;
                }
            }

            // Has the data changed within the threshhold of data being considered recent
            var hasDataChanged = _monthlyReleaseHelper.GetDateTimeNow()
                                     .Subtract(releaseDate).Days < newDataTimeSpanInDays;

            return new IndicatorDateChange
            {
                HasDataChangedRecently = hasDataChanged,
                DateOfLastChange = releaseDate
            };
        }
    }
}
