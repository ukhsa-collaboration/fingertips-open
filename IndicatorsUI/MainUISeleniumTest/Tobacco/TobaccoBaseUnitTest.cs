using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Skins;

namespace IndicatorsUI.MainUISeleniumTest.Tobacco
{
    [TestClass]
    public class TobaccoBaseUnitTest : BaseUnitTest
    {
        [TestInitialize]
        public override void CalledOnceAtStartOfTests()
        {
            SetSkin(SkinNames.Tobacco);
        }
    }
}
