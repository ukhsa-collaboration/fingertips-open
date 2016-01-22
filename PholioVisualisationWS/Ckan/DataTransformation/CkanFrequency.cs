using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public class CkanFrequency
    {
        public const string Annually = "Annually";
        public const string Quarterly = "Quarterly";
        public const string Monthly = "Monthly";

        public string Frequency { get; private set; }

        public CkanFrequency(TimePeriod timePeriod)
        {
            if (timePeriod.IsMonthly)
            {
                Frequency = Monthly;
            }
            else if (timePeriod.IsQuarterly)
            {
                Frequency = Quarterly;
            }
            else
            {
                Frequency = Annually;
            }
        } 
    }
}