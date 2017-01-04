using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class GroupRootNationalValuesBuilder : IndicatorDataBuilder<GroupRootNationalValues>
    {
        private GroupRootNationalValues groupRootNationalValues = new GroupRootNationalValues();
        public ChildAreaValuesBuilder ChildAreaValuesBuilder { get; set; }

        protected override IndicatorData IndicatorData
        {
            get { return groupRootNationalValues; }
        }

        public override GroupRootNationalValues GetIndicatorData(GroupRoot groupRoot,
            IndicatorMetadata metadata, IList<IArea> benchmarkAreas)
        {
            if (ChildAreaValuesBuilder == null)
            {
                throw new FingertipsException("ChildAreaValuesBuilder has not been assigned");
            }

            groupRootNationalValues = new GroupRootNationalValues();

            SetIndicatorData(groupRoot, metadata, benchmarkAreas);
            SetAreaValues(groupRoot);

            return groupRootNationalValues;
        }

        private void SetAreaValues(GroupRoot groupRoot)
        {
            IList<CoreDataSet> dataList = ChildAreaValuesBuilder.Build(groupRoot.GetNationalGrouping());

            dataList = new CoreDataSetFilter(dataList)
                .SelectWhereValueIsValid()
                .OrderBy(x => x.Value)
                .ToList();

            new CoreDataProcessor(null).TruncateList(dataList);

            MoveSignificanceValues(dataList);

            groupRootNationalValues.AreaValues = dataList.ToDictionary(x => x.AreaCode, x => x);
        }

        private static void MoveSignificanceValues(IList<CoreDataSet> dataList)
        {
            foreach (var coreDataSet in dataList)
            {
                coreDataSet.SignificanceAgainstOneBenchmark = coreDataSet.Significance.Values.First();
            }
        }
    }
}