using PholioVisualisation.DataAccess;

namespace FingertipsDataExtractionTool.AverageCalculator
{
    public class Calculator
    {
        private IGroupDataReader _groupReader;
        private IAreasReader _areasReader;

        public Calculator()
        {
            _groupReader = ReaderFactory.GetGroupDataReader();
            _areasReader = ReaderFactory.GetAreasReader();
        }

        public void Calculate()
        {
            var indicators = _groupReader.GetAllIndicators();

            foreach (var indicator in indicators)
            {
                var groupings = _groupReader.GetGroupingsByIndicatorId(indicator);

                foreach (var grouping in groupings)
                {
                    var parentArea = _areasReader.GetParentCodesFromChildAreaId(grouping.AreaTypeId);
                    
                }
            }
        }
    }
}
