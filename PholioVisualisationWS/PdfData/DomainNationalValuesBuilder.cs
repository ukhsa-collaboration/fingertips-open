using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class DomainNationalValuesBuilder : DomainDataBuilder
    {
        private DomainNationalValues domainNationalValues;

        private GroupRootNationalValuesBuilder builder;

        public List<DomainNationalValues> GetDomainDataForProfile(int profileId, 
            int childAreaTypeId, IList<string> benchmarkAreaCodes)
        {
            InitBuilder(childAreaTypeId);

            return BuildDomainDataForProfile(profileId, childAreaTypeId, benchmarkAreaCodes)
                .ConvertAll(x => (DomainNationalValues)x);
        }

        private void InitBuilder(int childAreaTypeId)
        {
            builder = new GroupRootNationalValuesBuilder();

            var indicatorComparer = new IndicatorComparerFactory
            {
                PholioReader = ReaderFactory.GetPholioReader()
            };

            var listBuilder = new ChildAreaValuesBuilder(indicatorComparer,
                ReaderFactory.GetGroupDataReader(), ReaderFactory.GetAreasReader(),
                ReaderFactory.GetProfileReader())
            {
                ParentAreaCode = AreaCodes.England,
                AreaTypeId = childAreaTypeId
            };

            builder.ChildAreaValuesBuilder = listBuilder;
        }

        protected override DomainData NewDomainData()
        {
            domainNationalValues = new DomainNationalValues();
            return domainNationalValues;
        }

        protected override void AddIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata, 
            IList<IArea> benchmarkAreas)
        {
            domainNationalValues.IndicatorData.Add(
                builder.GetIndicatorData(groupRoot, metadata, benchmarkAreas));
        }
    }
}
