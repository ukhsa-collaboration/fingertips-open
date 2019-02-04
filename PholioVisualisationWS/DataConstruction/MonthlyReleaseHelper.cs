using PholioVisualisation.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface IMonthlyReleaseHelper
    {
        DateTime GetFollowingReleaseDate(DateTime givenDate);
        DateTime GetReleaseDate(int newDataDeploymentCount);

        /// <summary>
        /// Wrapper for DateTime.Now to allow unit testing
        /// </summary>
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

            var datesAfter = releaseDates.Where(x => x.ReleaseDate > givenDate).ToList();

            if (datesAfter.Any() == false)
            {
                throw new FingertipsException("No more release dates. Add some more to the MonthlyRelease table.");
            }

            return datesAfter.First().ReleaseDate;
        }

        public DateTime GetReleaseDate(int newDataDeploymentCount)
        {
            IEnumerable<MonthlyRelease> releaseDates = _repository.GetPastReleaseDates();

            if (newDataDeploymentCount > 0)
            {
                releaseDates = releaseDates.Take(newDataDeploymentCount);
            }

            return releaseDates.Last().ReleaseDate;
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
