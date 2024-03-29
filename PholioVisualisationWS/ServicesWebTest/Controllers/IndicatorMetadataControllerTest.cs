﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class IndicatorMetadataControllerTest
    {
        public string IndicatorId1 = IndicatorIds.Aged0To4Years.ToString();
        public string IndicatorId2 = IndicatorIds.ExcessWinterDeaths.ToString();

        [TestMethod]
        public void TestGetIndicatorMetadataTextProperties()
        {
            var properties = new IndicatorMetadataController().GetIndicatorMetadataTextProperties();
            Assert.IsTrue(properties.Select(x => x.ColumnName)
                .Contains(IndicatorMetadataTextColumnNames.DataSource));
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Indicator_That_Does_Not_Exist()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileForIndicatorList(
                IndicatorIds.IndicatorThatDoesNotExist.ToString());
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Single_Indicator_With_Only_Indicator_Id()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileForIndicatorList(
                IndicatorId1);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Single_Indicator_With_Indicator_Id_And_Profile_Id()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileForIndicatorList(
                IndicatorId2, ProfileIds.Phof);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Multiple_Indicators()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileForIndicatorList(
                IndicatorId1 + "," + IndicatorId2);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Profile()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileByProfile(ProfileIds.HealthProfiles);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_Metadata_For_Group()
        {
            var response = new IndicatorMetadataController().GetIndicatorMetadataFileByProfileGroup(
                GroupIds.HealthProfiles_OurCommunities);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Test_Get_All_Metadata()
        {
            var indicatorIdToMetadata = new IndicatorMetadataController().GetAllIndicatorMetadata();
            Assert.IsNotNull(indicatorIdToMetadata);
            Assert.IsTrue(indicatorIdToMetadata.Count > 0);
        }

        [TestMethod]
        public void Test_Get_Indicator_Names_By_GroupId()
        {
            var response = new IndicatorMetadataController().GetIndicatorNamesByGroupId(GroupIds.Phof_WiderDeterminantsOfHealth.ToString());
            Assert.IsNotNull(response);
        }
    }
}
