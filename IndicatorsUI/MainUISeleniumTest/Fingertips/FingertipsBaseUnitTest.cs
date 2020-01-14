using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.MainUI.Skins;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsBaseUnitTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize_FingertipsBase()
        {
            SetSkin(SkinNames.Core);
        }
    }
}
