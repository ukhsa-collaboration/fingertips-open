using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticeValueCollectionBuilder
    {
        public int PracticeYear { get; set; }
        public string TopAreaCode { get; set; }
        public string ParentAreaCode { get; set; }
        public string AreaCode { get; set; }
        public int DataPointOffset { get; set; }

        public int GroupId1 { get; set; }
        public int GroupId2 { get; set; }
        public int IndicatorId1 { get; set; }
        public int IndicatorId2 { get; set; }
        public int SexId1 { get; set; }
        public int SexId2 { get; set; }
        public int AgeId1 { get; set; }
        public int AgeId2 { get; set; }

        private IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
        private PracticeValueCollection collection = new PracticeValueCollection();
        private IndicatorMetadataRepository indicatorMetadataRepository = IndicatorMetadataRepository.Instance;

        public PracticeValueCollection Build(PracticeAxis axis1, PracticeAxis axis2)
        {
            bool isAreacode = !string.IsNullOrEmpty(AreaCode);

            // Indicator 1
            IndicatorMetadata indicatorMetadata1 = indicatorMetadataRepository.GetIndicatorMetadata(IndicatorId1);
            Grouping grouping1 = reader.GetGroupings(GroupId1, IndicatorId1, AreaTypeIds.GpPractice, SexId1, AgeId1).FirstOrDefault();
            TimePeriod period1 = new DataPointOffsetCalculator(grouping1, DataPointOffset, indicatorMetadata1.YearType).TimePeriod;

            if (axis1 == null)
            {
                axis1 = GetPracticeAxis(IndicatorId1, period1, SexId1, indicatorMetadata1);
                if (axis1 == null)
                {
                    return collection;
                }
            }
            collection.PracticeAxis1 = axis1;

            if (isAreacode)
            {
                CoreDataSet practiceData = reader.GetCoreData(grouping1, period1, AreaCode).FirstOrDefault();
                if (practiceData != null && practiceData.IsValueValid)
                {
                    collection.PracticeValue1 = practiceData.Value;
                }
            }

            // Indicator 2
            IndicatorMetadata indicatorMetadata2 = indicatorMetadataRepository.GetIndicatorMetadata(IndicatorId2);
            Grouping grouping2 = reader.GetGroupings(GroupId2, IndicatorId2, AreaTypeIds.GpPractice, SexId2, AgeId2).FirstOrDefault();
            TimePeriod period2 = new DataPointOffsetCalculator(grouping2, DataPointOffset, indicatorMetadata2.YearType).TimePeriod;

            if (axis2 == null)
            {
                axis2 = GetPracticeAxis(IndicatorId2, period2, SexId2, indicatorMetadata2);
                if (axis2 == null)
                {
                    return collection;
                }
            }
            collection.PracticeAxis2 = axis2;

            if (isAreacode)
            {
                CoreDataSet practiceData = reader.GetCoreData(grouping2, period2, AreaCode).FirstOrDefault();
                if (practiceData != null && practiceData.IsValueValid)
                {
                    collection.PracticeValue2 = practiceData.Value;
                }
            }

            AssignChildAreas(grouping1);

            return collection;
        }

        private void AssignChildAreas(Grouping grouping)
        {
            if (string.IsNullOrEmpty(ParentAreaCode) == false)
            {
                IAreasReader areaReader = ReaderFactory.GetAreasReader();
                collection.ChildAreaCodes = areaReader.GetChildAreaCodes(ParentAreaCode, grouping.AreaTypeId);
            }
        }

        private PracticeAxis GetPracticeAxis(int indicatorId, TimePeriod period, int sexId, IndicatorMetadata metadata)
        {
            PracticeAxis axis = new PracticeAxis();
            axis.IndicatorData = new PracticeDataAccess().GetPracticeCodeToValidValueMap(indicatorId, period, sexId);
            if (axis.IndicatorData.Count > 0)
            {
                axis.Limits = GetLimits(axis.IndicatorData);
                axis.Title = GetTitle(metadata);
            }
            return axis;
        }

        private string GetTitle(IndicatorMetadata metadata)
        {
            var name = metadata.Descriptive[IndicatorMetadataTextColumnNames.Name];

            string unitLabel = metadata.Unit.Label;
            if (string.IsNullOrWhiteSpace(unitLabel) == false)
            {
                unitLabel = " (" + metadata.Unit.Label + ")";

                // Do not append unit when it is included in the indicator name
                if (name.EndsWith(unitLabel) == false)
                {
                    return name + unitLabel;
                }
            }

            return name;
        }

        private static Limits GetLimits(Dictionary<string, float> data)
        {
            float min = data.Values.Min<float>();
            float max = data.Values.Max<float>();
            return new MinMaxRounder(min, max).Limits;
        }

    }
}
