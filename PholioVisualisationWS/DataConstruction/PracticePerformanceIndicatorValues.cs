using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticePerformanceIndicatorValues
    {
        public const string Qof = "Qof";
        public const string PatientsThatWouldRecommendPractice = "Recommend";
        public const string LifeExpectancyMale = "LifeExpectancyMale";
        public const string LifeExpectancyFemale = "LifeExpectancyFemale";

        public Dictionary<string, ValueData> IndicatorToValue { get; set; }

        private IGroupDataReader _groupDataReader;
        private int _dataPointOffset;

        public PracticePerformanceIndicatorValues(IGroupDataReader groupDataReader, string practiceCode, int dataPointOffset)
        {
            IndicatorToValue = new Dictionary<string, ValueData>();
            IndicatorToValue = new Dictionary<string, ValueData>();
            this._groupDataReader = groupDataReader;
            this._dataPointOffset = dataPointOffset;

            // Require count and denominator for QOF
            SetCoreData(GetData(IndicatorIds.QofPoints, practiceCode), Qof);
            SetValueData(GetData(IndicatorIds.PatientsThatWouldRecommendPractice, practiceCode),
                PatientsThatWouldRecommendPractice);
            SetLifeExpectancies(practiceCode);
        }

        private void SetLifeExpectancies(string practiceCode)
        {
            // Life Expectancy
            IEnumerable<CoreDataSet> life = GetYearOrderedData(IndicatorIds.LifeExpectancyMsoaBasedEstimate, practiceCode);
            if (life.Any())
            {
                const int maleId = SexIds.Male;
                const int femaleId = SexIds.Female;

                var dataList = life.Where(x => x.SexId == maleId).ToList();
                SetValueData(GetDataByOffset(dataList), LifeExpectancyMale);

                dataList = life.Where(x => x.SexId == femaleId).ToList();
                SetValueData(GetDataByOffset(dataList), LifeExpectancyFemale);
            }
        }

        private IList<CoreDataSet> GetYearOrderedData(int indicatorId, string practiceCode)
        {
            return _groupDataReader.GetCoreData(indicatorId, practiceCode).OrderBy(x => x.Year).ToList();
        }

        private CoreDataSet GetData(int indicatorId, string practiceCode)
        {
            var dataList = GetYearOrderedData(indicatorId, practiceCode);
            return GetDataByOffset(dataList);
        }

        private CoreDataSet GetDataByOffset(IList<CoreDataSet> dataList)
        {
            int maximumIndex = dataList.Count() - 1;

            if (maximumIndex < _dataPointOffset)
            {
                // Run out of data
                return null;
            }

            return dataList.ElementAt(maximumIndex - _dataPointOffset);
        }

        private void SetValueData(CoreDataSet data, string key)
        {
            if (data != null && data.IsValueValid)
            {
                var valueData = data.GetValueData();
                valueData.ValueFormatted = NumericFormatter.Format1DP(data.Value);
                IndicatorToValue[key] = valueData;
            }
        }

        private void SetCoreData(CoreDataSet data, string key)
        {
            if (data != null && data.IsValueValid)
            {
                IndicatorToValue[key] = data;
            }
        }
    }
}