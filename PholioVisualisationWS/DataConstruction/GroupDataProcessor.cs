
using System;
using System.Collections.Generic;
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
        private TargetComparerProvider _targetComparerProvider;

        public GroupDataProcessor(TargetComparerProvider targetComparerProvider)
        {
            _targetComparerProvider = targetComparerProvider;
        }

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
                    foreach (GroupRoot groupRoot in data.GroupRoots)
                    {
                        IndicatorMetadata metadata = data.GetIndicatorMetadataById(groupRoot.IndicatorId);
                        FormatDataAndStats(groupRoot, metadata, timePeriodFormatter);

                        new GroupRootComparisonManager
                        {
                            PholioReader = pholioReader,
                            TargetComparer = _targetComparerProvider.GetTargetComparer(metadata, groupRoot.FirstGrouping)
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

        private void FormatDataAndStats(GroupRoot groupRoot, IndicatorMetadata metadata,
            TimePeriodFormatter timePeriodFormatter)
        {
            var formatter = new NumericFormatterFactory(groupDataReader).New(metadata);

            new GroupRootFormatter().
                Format(groupRoot, metadata, timePeriodFormatter, formatter);
        }
    }
}