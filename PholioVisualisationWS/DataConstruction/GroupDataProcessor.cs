
using System;
using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupDataProcessor
    {
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public void Process(GroupData data)
        {
            Process(data, new DataPointTimePeriodFormatter());
        }

        public void Process(GroupData data, TimePeriodFormatter timePeriodFormatter)
        {
            if (data.IsDataOk)
            {
                try
                {
                    var england = AreaFactory.NewArea(areasReader, AreaCodes.England);

                    foreach (GroupRoot groupRoot in data.GroupRoots)
                    {
                        IndicatorMetadata metadata = data.GetIndicatorMetadataById(groupRoot.IndicatorId);
                        FormatDataAndStats(groupRoot, metadata, timePeriodFormatter);

                        new GroupRootComparisonManager
                        {
                            PholioReader = pholioReader,
                            TargetComparer = GetTargetComparer(metadata, groupRoot, england)
                        }
                        .CompareToCalculateSignficance(groupRoot, metadata);
                        new CoreDataProcessor(null).TruncateList(groupRoot.Data);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.LogException(ex, "");
                }
            }
        }

        private TargetComparer GetTargetComparer(IndicatorMetadata metadata, GroupRoot groupRoot, IArea england)
        {
            var targetComparer = TargetComparerFactory.New(metadata.TargetConfig);

            // Initialise the target comparer
            if (targetComparer as BespokeTargetPercentileRangeComparer != null)
            {
                new TargetComparerHelper(groupDataReader, england)
                    .GetPercentileData(targetComparer, groupRoot.FirstGrouping, metadata);              
            }
            else
            {
                new TargetComparerHelper(groupDataReader, england)
                    .AssignExtraDataIfRequired(england, targetComparer, groupRoot.FirstGrouping, metadata);
            }

            return targetComparer;
        }

        private void FormatDataAndStats(GroupRoot groupRoot, IndicatorMetadata metadata,
            TimePeriodFormatter timePeriodFormatter)
        {
            var formatter = NumericFormatterFactory.New(metadata, groupDataReader);

            new GroupRootFormatter().
                Format(groupRoot, metadata, timePeriodFormatter, formatter);
        }
    }
}