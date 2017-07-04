using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class PracticeAreaValuesBuilderTest
    {
        [TestMethod]
        public void TestBuildForPct()
        {
            Grouping grouping = new Grouping
            {
                AgeId = (int)AgeIds.Over18,
                SexId = SexIds.Persons,
                IndicatorId = IndicatorIds.PatientsThatWouldRecommendPractice,
                GroupId = 2000002,
                ComparatorMethodId = ComparatorMethodIds.SingleOverlappingCIs
            };

            PracticeAreaValuesBuilder builder = new PracticeAreaValuesBuilder(ReaderFactory.GetGroupDataReader())
            {
                AreaTypeId = AreaTypeIds.GpPractice,
                DataPointOffset = 0,
                ParentAreaCode = AreaCodes.Pct_Luton,
            };

            var hash = builder.Build(grouping);

            Assert.IsNotNull(hash);
        }
    }
}
