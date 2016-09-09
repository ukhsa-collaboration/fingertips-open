
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class IndicatorStatsResponse
    {
        public int IID { get; set; }
        public IndicatorStatsPercentiles Stats { get; set; }
        public IndicatorStatsPercentilesFormatted StatsF { get; set; }
        public bool? HaveRequiredValues { get; set; }
    }

    public class JsonBuilderIndicatorStats : JsonBuilderBase
    {
        private IndicatorStatsParameters _parameters;

        private IndicatorMetadataRepository indicatorMetadataRepository = IndicatorMetadataRepository.Instance;
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();
        private IndicatorStatsProcessor indicatorStatsProcessor = new IndicatorStatsProcessor();

        private IArea parentArea;
        private CcgPopulation ccgPopulation;
        private IList<string> areaCodesToIgnore;
        private IList<int> profileIds;

        private bool? doEnoughAreasHaveValues;

        private int childAreaCount;


        public JsonBuilderIndicatorStats(HttpContextBase context)
            : base(context)
        {
            _parameters = new IndicatorStatsParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderIndicatorStats(IndicatorStatsParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var responseObjects = GetIndicatorStats();
            return JsonConvert.SerializeObject(responseObjects);
        }

        public Dictionary<int, IndicatorStatsResponse> GetIndicatorStats()
        {
            profileIds = _parameters.RestrictResultsToProfileIdList;
            parentArea = AreaFactory.NewArea(areasReader, _parameters.ParentAreaCode);

            var roots = GetRoots();
            var responseObjects = new Dictionary<int, IndicatorStatsResponse>();

            SetAreaCodesToIgnore();

            if (parentArea.IsCcg)
            {
                ccgPopulation = new CcgPopulationProvider(pholioReader).GetPopulation(parentArea.Code);
            }

            childAreaCount = new ChildAreaCounter(areasReader)
                .GetChildAreasCount(parentArea, _parameters.ChildAreaTypeId);

            int rootIndex = 0;
            foreach (var root in roots)
            {
                Grouping grouping = root.Grouping[0];
                IndicatorMetadata metadata = indicatorMetadataRepository.GetIndicatorMetadata(grouping);
                TimePeriod timePeriod = new DataPointOffsetCalculator(grouping, _parameters.DataPointOffset, metadata.YearType).TimePeriod;

                IEnumerable<double> values = GetValuesForStats(grouping, timePeriod);

                IndicatorStatsResponse statsAndStatF;
                if (values != null)
                {
                    IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsCalculator(values).GetStats();
                    var formatter = NumericFormatterFactory.New(metadata, groupDataReader);
                    indicatorStatsProcessor.Truncate(statsPercentiles);

                    statsAndStatF = new IndicatorStatsResponse
                    {
                        IID = metadata.IndicatorId,
                        Stats = statsPercentiles,
                        StatsF = formatter.FormatStats(statsPercentiles),
                        HaveRequiredValues = doEnoughAreasHaveValues
                    };
                }
                else
                {
                    // No stats calculated
                    statsAndStatF = new IndicatorStatsResponse
                    {
                        IID = metadata.IndicatorId,
                        HaveRequiredValues = doEnoughAreasHaveValues
                    };
                }
                responseObjects[rootIndex] = statsAndStatF;

                rootIndex++;
            }

            return responseObjects;
        }

        private void SetAreaCodesToIgnore()
        {
            var profileId = _parameters.ProfileId;
            areaCodesToIgnore = profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredForSpineChart;
        }

        private IEnumerable<GroupRoot> GetRoots()
        {
            IGroupDataReader reader = ReaderFactory.GetGroupDataReader();
            IList<Grouping> groupings;
            if (_parameters.UseIndicatorIds)
            {
                groupings = new GroupingListProvider(reader, profileReader).GetGroupings(
                    profileIds,
                    _parameters.IndicatorIds,
                    _parameters.ChildAreaTypeId);

                var roots = new GroupRootBuilder().BuildGroupRoots(groupings);
                return new GroupRootFilter(reader).RemoveRootsWithoutChildAreaData(roots);
            }

            groupings = reader.GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(_parameters.GroupId, _parameters.ChildAreaTypeId);
            return new GroupRootBuilder().BuildGroupRoots(groupings);
        }

        private IEnumerable<double> GetValuesForStats(Grouping grouping, TimePeriod timePeriod)
        {
            IList<CoreDataSet> data = null;
            IList<double> values;

            if (parentArea.IsCountry)
            {
                // Optimisation for large number of areas
                values = groupDataReader.GetOrderedCoreDataValidValuesForAllAreasOfType(grouping, timePeriod,
                    areaCodesToIgnore);
            }
            else
            {
                data = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping, parentArea, timePeriod);
                data = new CoreDataSetFilter(data).RemoveWithAreaCode(areaCodesToIgnore).ToList();
                data = data.OrderBy(x => x.Value).ToList();
                values = new ValueListBuilder(data).ValidValues;
            }

            // Apply rules
            int areaTypeId = grouping.AreaTypeId;
            if (areaTypeId != AreaTypeIds.GpPractice)
            {
                doEnoughAreasHaveValues = IsRequiredNumberOfAreaValues(values) ||
                    ShouldSpineChartAlwaysBeAvailable(grouping.IndicatorId);

                if (doEnoughAreasHaveValues == false)
                {
                    values = null;
                }
            }
            else if (parentArea.IsCcg)
            {
                // CCG average of GP practices
                if (RuleShouldCcgAverageBeCalculated.Validates(grouping, data, ccgPopulation) == false)
                {
                    values = null;
                }
            }

            return values;
        }

        /// <summary>
        /// See FIN-1084: spine chart should be displayed for SSI indicator
        /// </summary>
        private bool ShouldSpineChartAlwaysBeAvailable(int indicatorId)
        {
            return indicatorId == IndicatorIds.SurgicalSiteInfectionHipProsthesis ||
                   indicatorId == IndicatorIds.SurgicalSiteInfectionKneeProsthesis;
        }

        /// <summary>
        /// Rule that values must be available for at least 75% of the child areas.
        /// </summary>
        private bool IsRequiredNumberOfAreaValues(IEnumerable<double> values)
        {
            if (childAreaCount > 0)
            {
                double fractionOfAreasWithValues = Convert.ToDouble(values.Count()) /
                    Convert.ToDouble(childAreaCount);

                return fractionOfAreasWithValues >= 0.75;
            }

            return false;
        }
    }
}