using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticeSummary
    {
        private readonly IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();

        public PracticeSummary(IGroupDataReader groupDataReader, IAreasReader areasReader)
        {
            _areasReader = areasReader;
            _groupDataReader = groupDataReader;
        }

        public int? GetDeprivationDecile(string areaCode)
        {
            var decile = _areasReader.GetCategorisedArea(areaCode, AreaTypeIds.Country, AreaTypeIds.GpPractice,
                CategoryTypeIds.DeprivationDecileGp2019);

            if (decile == null)
            {
                return null;
            }

            return decile.CategoryId;
        }

        public string GetEthnicityText(string areaCode, int groupId)
        {
            const int categoryTypeId = EthnicityLabelBuilder.EthnicityCategoryTypeId;

            Grouping grouping = _groupDataReader.GetGroupingsByGroupIdAndIndicatorId(groupId,
                IndicatorIds.EthnicityEstimates);

            if (grouping == null)
            {
                throw new FingertipsException("Ethnicity estimates not found in practice profiles supporting indicators domain");
            }

            var dataList = _groupDataReader.GetCoreDataListForAllCategoryAreasOfCategoryAreaType(
                grouping, TimePeriod.GetDataPoint(grouping),
                categoryTypeId, areaCode);

            if (dataList.Any())
            {
                var categories = _areasReader.GetCategories(categoryTypeId);

                EthnicityLabelBuilder builder = new EthnicityLabelBuilder(dataList, categories);
                if (builder.IsLabelRequired)
                {
                    return builder.Label;
                }
            }

            return null;
        }
    }
}