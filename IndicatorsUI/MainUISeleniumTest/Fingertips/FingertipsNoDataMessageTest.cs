using System;
using System.Threading;
using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsNoDataMessageTest : FingertipsBaseUnitTest
    {
        private const string NoIndicators = "No indicators for current area type";
        private const string NotApplicable = "Not applicable for England data";

        private string[] _tabsToVisit;

        [TestInitialize]
        public void TestInitialize()
        {
            _tabsToVisit = new []{  FingertipsIds.TabOverview,
                                        FingertipsIds.TabCompareIndicators,
                                        FingertipsIds.TabMap,
                                        FingertipsIds.TabTrends,
                                        FingertipsIds.TabCompareAreas,
                                        FingertipsIds.TabAreaProfiles,
                                        FingertipsIds.TabInequalities,
                                        FingertipsIds.TabEngland,
                                        FingertipsIds.TabBoxPlot,
                                        FingertipsIds.TabDefinitions,
                                        FingertipsIds.TabDownload
            };

            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.DevelopmentProfileForTesting);
            fingertipsHelper.SelectDomainWithText("no data for any area type");
        }

        [TestMethod]
        public void Test_No_Indicator_Data_Message_For_England_Area_Type_On_All_Tabs()
        {
            foreach (var tabId in _tabsToVisit)
            {
                var message = GetMessageForTabAndSelectAreaType(tabId, AreaTypeIds.England);
                var expectedMessage = GetExpectedMessageForNoDataInEngland(tabId);
                Assert.AreEqual(expectedMessage, message,  
                    string.Format("{0} {1}:{2} <{3}> {4} <{5}>", "For the page", tabId, 
                        "The message is expected is", expectedMessage, "and it was", message));
            }
        }

        [TestMethod]
        public void Test_No_Indicator_Data_Message_For_Non_England_Area_Type_On_All_Tabs()
        {
            foreach (var tabId in _tabsToVisit)
            {
                var message = GetMessageForTabAndSelectAreaType(tabId, AreaTypeIds.DistrictAndUAPreApr2019);
                Assert.AreEqual(NoIndicators, message, "The message is unexpected");
            }
        }

        private string GetMessageForTabAndSelectAreaType(string pageId, int areaTypeId)
        {
            // Select tab and area type
            fingertipsHelper.SelectAreaType(areaTypeId);
            fingertipsHelper.SelectTab(pageId);

            // Wait for message to be ready
            var messageId = "main-info-message";
            waitFor.ExpectedElementToBeVisible(By.Id(messageId));

            // Return message
            var messageTag = fingertipsHelper.FindElementById(messageId);
            return messageTag.Text;
        }

        private string GetExpectedMessageForNoDataInEngland(string pageId)
        {


            switch (pageId)
            {
                case FingertipsIds.TabOverview: return NoIndicators;
                case FingertipsIds.TabCompareIndicators: return NotApplicable;
                case FingertipsIds.TabMap: return NotApplicable;
                case FingertipsIds.TabTrends: return NoIndicators;
                case FingertipsIds.TabCompareAreas: return NotApplicable;
                case FingertipsIds.TabAreaProfiles: return NoIndicators;
                case FingertipsIds.TabInequalities: return NoIndicators;
                case FingertipsIds.TabEngland: return NoIndicators;
                case FingertipsIds.TabBoxPlot: return NotApplicable;
                case FingertipsIds.TabDefinitions: return NoIndicators;
                case FingertipsIds.TabDownload: return NoIndicators;
                default:
                    throw new Exception("Sorry,there is no page with this numeration");
            }
        }

    }
}