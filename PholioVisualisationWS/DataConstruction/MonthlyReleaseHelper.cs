using PholioVisualisation.DataAccess.Repositories;
using System;
using System.Linq;

namespace PholioVisualisation.DataConstruction
{
    public interface IMonthlyReleaseHelper
    {
        DateTime GetFollowingReleaseDate(DateTime givenDate);
        DateTime GetDateTimeNow();
    }

    public class MonthlyReleaseHelper : IMonthlyReleaseHelper
    {
        private MonthlyReleaseRepository _repository;

        public MonthlyReleaseHelper()
        {
            _repository = new MonthlyReleaseRepository();
        }

        public DateTime GetFollowingReleaseDate(DateTime givenDate)
        {
            var releaseDates = _repository.GetReleaseDates();
            var latestReleaseDate = releaseDates.Where(x => x.ReleaseDate > givenDate).First();
            return latestReleaseDate.ReleaseDate;
        }

        /// <summary>
        /// Wrapper for DateTime.Now to allow unit testing
        /// </summary>
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }
    }
}
