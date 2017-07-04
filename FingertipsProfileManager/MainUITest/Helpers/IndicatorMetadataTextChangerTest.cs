using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class IndicatorMetadataTextChangerTest
    {
        private const int IndicatorId = IndicatorIds.ChildrenInPoverty;
        private const int ProfileId = ProfileIds.Phof;
        private const string Text = "test name";
        private const string UserName = UserNames.Doris;

        private IndicatorMetadataTextChanger _indicatorMetadataTextChanger;

        [TestInitialize]
        public void TestInitialize()
        {
            _indicatorMetadataTextChanger = new IndicatorMetadataTextChanger(new ProfileRepository(), GetProfilesReader());
        }

        [TestMethod]
        public void TestPropertyCanBeOverridden()
        {
            var item = new IndicatorMetadataTextItem
            {
                IsBeingOverriddenForFirstTime = true,
                PropertyId = IndicatorTextMetadataPropertyIds.Name,
                Text = Text
            };

            var property = GetIndicatorMetadataTextProperty();

            // Act: set the overridden text
            _indicatorMetadataTextChanger.UpdateIndicatorTextValues(IndicatorId,
                new List<IndicatorMetadataTextItem> { item }, new List<IndicatorMetadataTextProperty> { property },
                UserName, ProfileId);

            // Assert: check name has been overridden
            IndicatorText indicatorText = GetProfilesReader().GetIndicatorTextValues(IndicatorId,
             new List<IndicatorMetadataTextProperty> { property },
             ProfileId).First();
            Assert.AreEqual(Text, indicatorText.ValueSpecific);
        }

        private static IndicatorMetadataTextProperty GetIndicatorMetadataTextProperty()
        {
            var property = new IndicatorMetadataTextProperty
            {
                PropertyId = IndicatorTextMetadataPropertyIds.Name,
                ColumnName = IndicatorTextMetadataColumnNames.Name
            };
            return property;
        }

        private static ProfilesReader GetProfilesReader()
        {
            return ReaderFactory.GetProfilesReader();
        }
    }
}
