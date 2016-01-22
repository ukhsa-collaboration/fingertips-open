using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Skins;

namespace MainUISeleniumTest.Tobacco
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
