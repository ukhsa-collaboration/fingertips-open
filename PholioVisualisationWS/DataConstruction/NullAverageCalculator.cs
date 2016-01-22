using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class NullAverageCalculator : AverageCalculator
    {
        public override CoreDataSet Average
        {
            get
            {
                return null;
            }
        }
    }
}