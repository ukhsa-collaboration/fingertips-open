using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Exceptions;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class IndicatorMetadataTextCreatorTest
    {
        private const string IndicatorText = "new test indicator";
        private const string UserName = UserNames.Doris;
        private const int ProfileId = ProfileIds.Phof;
        private int _indicatorId;
        private IndicatorMetadataTextCreator _indicatorMetadataTextCreator;
        private IList<IndicatorMetadataTextProperty> _indicatorMetadataTextProperties;

        [TestInitialize]
        public void TestInitialize()
        {
            var repo = new ProfileRepository(NHibernateSessionFactory.GetSession());
            _indicatorId = repo.GetNextIndicatorId();
            _indicatorMetadataTextCreator = new IndicatorMetadataTextCreator(repo);
            _indicatorMetadataTextProperties = IndicatorMetadataTextProperties();
        }

        [TestMethod]
        public void TestCreateNew()
        {
            var indicatorMetadataTextItems = GetMandatoryIndicatorMetadataTextItems(
                _indicatorMetadataTextProperties);

            // Act
            CreateIndicator(indicatorMetadataTextItems, _indicatorMetadataTextProperties);

            // Assert
            CheckIndicatorHasBeenCreated();
        }

        [TestMethod]
        public void TestCreateNewWhenLongStrings()
        {
            var indicatorMetadataTextItems = GetAllIndicatorMetadataTextItems(
                _indicatorMetadataTextProperties);

            SetLargeStringToProperty(indicatorMetadataTextItems, IndicatorTextMetadataPropertyIds.Definition);
            SetLargeStringToProperty(indicatorMetadataTextItems, IndicatorTextMetadataPropertyIds.Rationale);
            SetLargeStringToProperty(indicatorMetadataTextItems, IndicatorTextMetadataPropertyIds.Methodology);
            SetLargeStringToProperty(indicatorMetadataTextItems, IndicatorTextMetadataPropertyIds.Caveats);
            SetLargeStringToProperty(indicatorMetadataTextItems, IndicatorTextMetadataPropertyIds.Notes);

            // Act
            CreateIndicator(indicatorMetadataTextItems, _indicatorMetadataTextProperties);

            // Assert
            CheckIndicatorHasBeenCreated();
        }

        private void CheckIndicatorHasBeenCreated()
        {
            var textValues = ReaderFactory.GetProfilesReader()
                .GetIndicatorTextValues(_indicatorId, _indicatorMetadataTextProperties, ProfileId).ToList();
            Assert.AreEqual(IndicatorText, textValues.First().ValueGeneric);
        }

        private void CreateIndicator(List<IndicatorMetadataTextItem> indicatorMetadataTextItems,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            _indicatorMetadataTextCreator.CreateNewIndicatorTextValues(ProfileId,
                indicatorMetadataTextItems, indicatorMetadataTextProperties, _indicatorId, UserName);
        }

        private static IList<IndicatorMetadataTextProperty> IndicatorMetadataTextProperties()
        {
            var indicatorMetadataTextProperties = ReaderFactory.GetProfilesReader().GetIndicatorMetadataTextProperties();
            return indicatorMetadataTextProperties;
        }

        private static List<IndicatorMetadataTextItem> GetMandatoryIndicatorMetadataTextItems(
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            var indicatorMetadataTextItems = new List<IndicatorMetadataTextItem>();
            foreach (var indicatorMetadataTextProperty in indicatorMetadataTextProperties)
            {
                if (indicatorMetadataTextProperty.IsMandatory)
                {
                    indicatorMetadataTextItems.Add(new IndicatorMetadataTextItem
                    {
                        PropertyId = indicatorMetadataTextProperty.PropertyId,
                        Text = IndicatorText
                    });
                }
            }
            return indicatorMetadataTextItems;
        }

        private static List<IndicatorMetadataTextItem> GetAllIndicatorMetadataTextItems(
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            var indicatorMetadataTextItems = new List<IndicatorMetadataTextItem>();
            foreach (var indicatorMetadataTextProperty in indicatorMetadataTextProperties)
            {
                    indicatorMetadataTextItems.Add(new IndicatorMetadataTextItem
                    {
                        PropertyId = indicatorMetadataTextProperty.PropertyId,
                        Text = IndicatorText
                    });
            }

            // Data quality single character
            indicatorMetadataTextItems.First(x => x.PropertyId == IndicatorTextMetadataPropertyIds.DataQuality)
                .Text = "1";

            return indicatorMetadataTextItems;
        }

        /// <summary>
        /// Set property text to be larger than nvarchar(max) limit of 4000 chars
        /// </summary>
        private static void SetLargeStringToProperty(IList<IndicatorMetadataTextItem> indicatorMetadataTextItems,
            int propertyId)
        {
            indicatorMetadataTextItems.First(x => x.PropertyId == propertyId)
                .Text = new string('a', 5000);
        }
    }
}