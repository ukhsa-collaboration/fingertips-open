using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class ChildAreaCounter
    {
        private IAreasReader _areasReader;

        public ChildAreaCounter(IAreasReader areasReader)
        {
            _areasReader = areasReader;
        }

        public int GetChildAreasCount(IArea parentArea, int childAreaTypeId)
        {
            var categoryArea = parentArea as CategoryArea;
            if (categoryArea != null)
            {
                return _areasReader.GetChildAreaCount(categoryArea, childAreaTypeId);
            }

            return parentArea.IsCountry
                    ? _areasReader.GetAreaCountForAreaType(childAreaTypeId) // NOTE ignored areas not considered here
                    : _areasReader.GetChildAreaCount(parentArea.Code, childAreaTypeId);
        }
    }
}