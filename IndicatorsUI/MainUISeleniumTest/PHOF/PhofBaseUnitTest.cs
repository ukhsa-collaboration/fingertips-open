﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.MainUI.Skins;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofBaseUnitTest : BaseUnitTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            SetSkin(SkinNames.Phof);
        }
    }
}