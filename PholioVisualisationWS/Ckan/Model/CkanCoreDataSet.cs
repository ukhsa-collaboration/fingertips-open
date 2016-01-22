using PholioVisualisation.PholioObjects;

namespace Ckan.Model
{
    public class CkanCoreDataSet
    {
        public CoreDataSet Data { get; set; }

        public CkanCoreDataSet(CoreDataSet coreDataSet, string timePeriodString)
        {
            Data = coreDataSet;
            TimePeriodString = timePeriodString;
        }

        public string TimePeriodString { get; set; }
    }
}