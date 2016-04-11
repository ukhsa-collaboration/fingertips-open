﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class ReaderFactoryTest
    {
        [TestMethod]
        public void TestGetProfileReader()
        {
            var reader = ReaderFactory.GetProfileReader();
            Assert.IsNotNull(reader);
        }
    }
}
