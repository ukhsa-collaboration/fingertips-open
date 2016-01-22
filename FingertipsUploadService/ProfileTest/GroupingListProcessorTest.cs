using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Profile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ProfileDataTest
{
    [TestClass]
    public class GroupingListProcessorTest
    {
        [TestMethod]
        public void Test()
        {
            var groupings = new List<Grouping>
            {
                new Grouping{IndicatorId = 1},
                new Grouping{IndicatorId = 1},
                new Grouping{IndicatorId = 2}
            };

            new GroupingListProcessor().RecalculateSequences(groupings);

            Assert.AreEqual(1, groupings[0].Sequence);
            Assert.AreEqual(1, groupings[1].Sequence);
            Assert.AreEqual(2, groupings[2].Sequence);
        }
    }
}
