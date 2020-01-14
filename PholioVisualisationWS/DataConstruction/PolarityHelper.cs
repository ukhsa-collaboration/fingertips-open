using NHibernate;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public static class PolarityHelper
    {
        /// <summary>
        /// Whether or not the commonest polarity across all the groupings of an
        /// indicator should be used.
        /// </summary>
        public static bool ShouldUseCommonestPolarity(int profileId)
        {
            return profileId == ProfileIds.Search || profileId == ProfileIds.Undefined;
        }

        /// <summary>
        /// Gets the commonest polarity across all the groupings of an indicator
        /// </summary>
        public static int GetCommonestPolarityForIndicator(int indicatorId)
        {
            return ReaderFactory.GetGroupDataReader().GetCommonestPolarityForIndicator(indicatorId);
        }
    }
}