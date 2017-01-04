using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class SubnationalAreaDataWriter : ParentDataWriter
    {
        private readonly Dictionary<string, IArea> _subnationalAreaCodeToAreaMap;
        private readonly IList<IArea> _subnationalAreas;
        private readonly string[] _subnationalAreaCodes;
        private readonly ChildAreaListBuilder _childAreaListBuilder;

        public SubnationalAreaDataWriter(IAreasReader areasReader, IGroupDataReader groupDataReader, WorksheetInfo worksheet,
            ProfileDataWriter profileDataWriter, IAreaType subnationalAreaType)
                        : base(areasReader, groupDataReader, worksheet, profileDataWriter)
        {
            _childAreaListBuilder = new ChildAreaListBuilder(AreasReader);

            // Subnational areas
            _subnationalAreas = areasReader.GetAreasByAreaTypeId(subnationalAreaType.Id);
            _subnationalAreaCodes = _subnationalAreas.Select(x => x.Code).ToArray();
            _subnationalAreaCodeToAreaMap = _subnationalAreas
                .ToDictionary<IArea, string, IArea>(
                area => area.Code,
                area => area
                );
        }

        public override IList<CategoryIdAndAreaCode> CategoryIdAndAreaCodes
        {
            get
            {
                return _subnationalAreaCodes.OrderBy(x => x)
                    .Select(x => new CategoryIdAndAreaCode { AreaCode = x })
                    .ToList();
            }
        }

        public override IList<CoreDataSet> AddMultipleAreaData(RowLabels rowLabels, Grouping grouping,
            TimePeriod timePeriod, IndicatorMetadata metadata, Dictionary<string, Area> areaCodeToParentMap)
        {
            var dataList = GroupDataReader.GetCoreData(grouping, timePeriod,
                _subnationalAreaCodes);

            if (dataList.Any())
            {
                ProfileDataWriter.AddData(Worksheet, rowLabels, dataList,
                    _subnationalAreaCodeToAreaMap, areaCodeToParentMap);
            }
            else
            {
                dataList = AddCalculatedAverageValue(Worksheet, rowLabels, _subnationalAreas,
                    grouping, timePeriod, metadata);
            }

            return dataList;
        }

        private IList<CoreDataSet> AddCalculatedAverageValue(WorksheetInfo ws, RowLabels rowLabels, IList<IArea> subnationalAreas,
            Grouping grouping, TimePeriod timePeriod, IndicatorMetadata metadata)
        {
            IList<CoreDataSet> dataList = new List<CoreDataSet>();
            if (subnationalAreas != null)
            {
                foreach (var subnationalArea in subnationalAreas)
                {
                    var coreData = new SubnationalAreaAverageCalculator(GroupDataReader, _childAreaListBuilder)
                        .CalculateAverage(grouping, timePeriod, metadata, subnationalArea);
                    dataList.Add(coreData);
                    if (coreData != null)
                    {
                        ProfileDataWriter.AddData(ws, rowLabels, coreData, subnationalArea);
                    }
                }
            }
            return dataList;
        }
    }
}