namespace Fpm.MainUI.ViewModels.ProfilesAndIndicators
{
    public class BrowseDataViewModel
    {
        public int IndicatorId { get; set; }

        public int AreaTypeId { get; set; }

        public int SexId { get; set; }

        public int AgeId { get; set; }

        public int YearRange { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Quarter { get; set; }

        public int CategoryTypeId { get; set; }

        public CoreDataSetViewModel Results { get; set; }

      }
}