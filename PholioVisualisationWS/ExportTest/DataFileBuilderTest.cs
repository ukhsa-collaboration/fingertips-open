using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class DataFileBuilderTest
    {
        private IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private byte[] _phofBytes;

        [TestMethod]
        public void TestRagNoProfile()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Undefined
            };

            var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                GetExportAreaHelper(parameters), _areasReader, new FileBuilder());

            var bytes = fileBuilder.GetFileForSpecifiedIndicators(
                GetPhofIndicatorIds(), parameters);

            // Assert
            var csv = GetCsv(bytes);
            Assert.IsTrue(csv.Contains("Derby"));
        }

        [TestMethod]
        public void TestRagPhof()
        {
            // Assert
            var csv = GetCsv(GetBytesForPhof());
            Assert.IsTrue(csv.Contains("Derby"));
        }

        [TestMethod]
        public void TestSortableDataAdded()
        {
            // Assert
            var csv = GetCsv(GetBytesForPhof());
            Assert.IsTrue(csv.Contains("20120000"));
        }

        [TestMethod]
        public void TestQuintiles()
        {

            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.Ccg,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.Subregion,
                ProfileId = ProfileIds.Amr
            };

            var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                GetExportAreaHelper(parameters), _areasReader, new FileBuilder());

            var bytes = fileBuilder.GetFileForSpecifiedIndicators(
                new List<int> { IndicatorIds.NumberPrescribedAntibioticItems }, parameters);

            // Assert
            var csv = GetCsv(bytes);
            Assert.IsTrue(csv.Contains("Derby"));
        }

        [TestMethod]
        public void TestStp()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.Stp,
                ParentAreaCode = AreaCodes.England,
                ParentAreaTypeId = AreaTypeIds.Country,
                ProfileId = ProfileIds.MentalHealthJsna
            };

            var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                GetExportAreaHelper(parameters), _areasReader, new FileBuilder());

            var bytes = fileBuilder.GetFileForSpecifiedIndicators(
                new List<int> { IndicatorIds.EstimatedPrevalenceCommonMentalHealthDisorders },
                parameters);

            // Assert
            var csv = GetCsv(bytes);
            Assert.IsTrue(csv.Contains("Derby"));
        }

        [TestMethod]
        public void TestIndicatorForUndefinedProfile()
        {
            var parameters = new IndicatorExportParameters
            {
                ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                ParentAreaCode = AreaCodes.Gor_EastMidlands,
                ParentAreaTypeId = AreaTypeIds.GoRegion,
                ProfileId = ProfileIds.Undefined
            };

            var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                GetExportAreaHelper(parameters), _areasReader, new FileBuilder());

            var bytes = fileBuilder.GetFileForSpecifiedIndicators(
                new List<int> { IndicatorIds.HealthyLifeExpectancyAtBirth },
                parameters);

            // Assert
            var csv = GetCsv(bytes);
            Assert.IsTrue(csv.Contains("Derby"));
        }

        private byte[] GetBytesForPhof()
        {
            if (_phofBytes == null)
            {

                var parameters = new IndicatorExportParameters
                {
                    ChildAreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority,
                    ParentAreaCode = AreaCodes.England,
                    ParentAreaTypeId = AreaTypeIds.GoRegion,
                    ProfileId = ProfileIds.Phof,
                    IncludeSortableTimePeriod = true
                };

                var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                    GetExportAreaHelper(parameters), _areasReader, new FileBuilder());


                _phofBytes = fileBuilder.GetFileForSpecifiedIndicators(
                    GetPhofIndicatorIds(), parameters);
            }

            return _phofBytes;
        }

        private static List<int> GetPhofIndicatorIds()
        {
            return new List<int>
            {
                IndicatorIds.VaccinationCoverageDatp,
                IndicatorIds.SmokingAtTimeOfDelivery
            };
        }

        private ExportAreaHelper GetExportAreaHelper(IndicatorExportParameters parameters)
        {
            return new ExportAreaHelper(_areasReader, parameters, new AreaFactory(_areasReader));
        }

        private string GetCsv(byte[] data)
        {
            var s = Encoding.UTF8.GetString(data);
            Console.Write(s);
            return s;
        }
    }
}
