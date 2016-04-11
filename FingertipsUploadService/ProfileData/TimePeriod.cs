namespace FingertipsUploadService.ProfileData
{
    public class TimePeriod
    {
        public const int Undefined = -1;

        public int Year = Undefined;
        public int Quarter = Undefined;
        public int Month = Undefined;
        public int YearRange = Undefined;

        public bool IsQuarterly
        {
            get { return Quarter > 0; }
        }

        public bool IsMonthly
        {
            get { return Month > 0; }
        }

        public bool IsLaterThan(TimePeriod period)
        {
            if (Year > period.Year)
            {
                return true;
            }

            if (Year < period.Year)
            {
                return false;
            }

            if (IsQuarterly)
            {
                return Quarter > period.Quarter;
            }

            if (IsMonthly)
            {
                return Month > period.Month;
            }

            return false;
        }
    }
}