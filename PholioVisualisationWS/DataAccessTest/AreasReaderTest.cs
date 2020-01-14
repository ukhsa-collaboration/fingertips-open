using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class AreasReaderTest
    {
        [TestMethod]
        public void TestGetAreaCodesThatDoNotExist_Where_Code_Does_Not_Exist()
        {
            var codes = Reader().GetAreaCodesThatDoNotExist(new List<string> {"zzz"});
            Assert.IsTrue(codes.Any());
        }

        [TestMethod]
        public void TestGetAreaCodesThatDoNotExist_Where_Code_Exists()
        {
            var codes = Reader().GetAreaCodesThatDoNotExist(new List<string> { AreaCodes.England });
            Assert.IsFalse(codes.Any());
        }

        [TestMethod]
        public void TestGetParentAreasFromChildAreaId()
        {
            var map = Reader().GetParentAreasFromChildAreaId(
                AreaTypeIds.GoRegion, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);

            // Assert: about 150 LAs
            Assert.IsTrue(map.Keys.Count > 140 && map.Keys.Count < 160);
        }

        [TestMethod]
        public void TestGetParentAreasForGpChildAreaId()
        {
            var map = Reader().GetParentAreasFromChildAreaId(
                AreaTypeIds.CcgsPostApr2019, AreaTypeIds.GpPractice);

            Assert.IsTrue(map.Keys.Any());
        }

        [TestMethod]
        public void TestGetParentAreaGroup()
        {
            IList<ParentAreaGroup> parentAreaGroups = Reader().GetParentAreaGroupsForProfile(ProfileIds.Undefined);
            Assert.IsTrue(parentAreaGroups.Count > 0);
        }

        [TestMethod]
        public void TestGetParentAreaGroupsForChildAreaType()
        {
            var areaTypeId = AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019;
            IList<ParentAreaGroup> parentAreaGroups = Reader().GetParentAreaGroupsForChildAreaType(areaTypeId);

            // Assert: all child areas match expected type ID
            Assert.IsTrue(parentAreaGroups.Count > 0);
            Assert.AreEqual(parentAreaGroups.Count, 
                parentAreaGroups.Where(x => x.ChildAreaTypeId == areaTypeId).Count());
        }

        [TestMethod]
        public void TestGetAllPlaceNames()
        {
            var placeNames = Reader().GetAllPlaceNames();
            Assert.IsTrue(placeNames.Count > 30000);
        }

        [TestMethod]
        public void TestGetAllPostCodeParentAreasStartingWithLetter()
        {
            var postcodes = Reader().GetAllPostCodeParentAreasStartingWithLetter("a");
            Assert.IsTrue(postcodes.Any());
            Assert.AreEqual('a', postcodes.First().Postcode.ToLower().First());

            postcodes = Reader().GetAllPostCodeParentAreasStartingWithLetter("s");
            Assert.IsTrue(postcodes.Any());
            Assert.AreEqual('s', postcodes.First().Postcode.ToLower().First());
        }

        [TestMethod]
        public void TestAllPlaceNameMapsToPostcodeParentAreas()
        {
            var placeNames = Reader().GetAllPlaceNames();
            foreach (var placeName in placeNames)
            {
                Assert.AreEqual(placeName.Postcode, placeName.PostcodeParentAreas.Postcode);
            }
        }

        [TestMethod]
        public void TestGetParentAreaGroup_DeprivationDecileRetrieved()
        {
            IList<ParentAreaGroup> parentAreaGroups = Reader().GetParentAreaGroupsForProfile(ProfileIds.Undefined);
            Assert.IsNotNull(parentAreaGroups.FirstOrDefault(x => x.CategoryTypeId.HasValue));
        }

        [TestMethod]
        public void TestGetParentAreaGroup_CategoryTypeIdAndParentTypeIdAreMutuallyExclusive()
        {
            IList<ParentAreaGroup> parentAreaGroups = Reader().GetParentAreaGroupsForProfile(ProfileIds.Undefined);

            foreach (ParentAreaGroup parentAreaGroup in parentAreaGroups)
            {
                int? parentAreaTypeId = parentAreaGroup.ParentAreaTypeId;
                int? categoryTypeId = parentAreaGroup.CategoryTypeId;

                Assert.IsTrue(
                    (parentAreaTypeId.HasValue && categoryTypeId.HasValue == false) ||
                    (parentAreaTypeId.HasValue == false && categoryTypeId.HasValue)
                    );
            }
        }

        [TestMethod]
        public void TestGetParentsFromChildAreaId()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            Dictionary<string, Area> map = reader.GetParentsFromChildAreaIdAndParentAreaTypeId(
                AreaTypeIds.Pct, AreaTypeIds.GpPractice);

            Assert.AreEqual(map["A81006"].Code, AreaCodes.Pct_StocktonOnTees);
        }

        [TestMethod]
        public void TestGetAreaFromCode()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            Assert.AreEqual("England", reader.GetAreaFromCode(AreaCodes.England).Name);
        }

        [TestMethod]
        public void TestGetAreaFromCodeThrowsExceptionIfAreaCodeNotInDatabase()
        {
            const string code = "NOT_EXIST";

            try
            {
                Area area = Reader().GetAreaFromCode(code);
            }
            catch (FingertipsException ex)
            {
                Assert.AreEqual(ex.Message, "Area cannot be found: '" + code + "'");
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestGetAreasFromCodes()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var codes = new[] { AreaCodes.Sha_EastOfEngland, AreaCodes.Sha_London };
            Assert.AreEqual(2, reader.GetAreasFromCodes(codes).Count);
        }

        [TestMethod]
        public void TestGetAreaWithAddressFromCode()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            AreaAddress area = reader.GetAreaWithAddressFromCode(AreaCodes.Gp_MeersbrookSheffield);

            Assert.AreEqual(AreaCodes.Gp_MeersbrookSheffield, area.Code);
            Assert.AreEqual("S8 0RT", area.Postcode);
        }

        [TestMethod]
        public void TestGetAreaWithAddressFromCode_EastingNorthingIsDefined()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            AreaAddress area = reader.GetAreaWithAddressFromCode(AreaCodes.Gp_MeersbrookSheffield);
            Assert.IsNotNull(area.EastingNorthing);
        }

        [TestMethod]
        public void TestGetAllAreaTypes()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var areaTypes = reader.GetAllAreaTypes();
            Assert.IsTrue(areaTypes.Count > 0);
        }

        [TestMethod]
        public void TestGetAreaType()
        {
            var id = AreaTypeIds.CcgsPostApr2019;
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var areaType = reader.GetAreaType(id);
            Assert.AreEqual(id, areaType.Id);
        }

        [TestMethod]
        public void TestGetAreaTypes()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            // Multiple area types
            Assert.AreEqual(2, reader.GetAreaTypes(new List<int> { AreaTypeIds.Sha, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 }).Count);

            // One area type
            AreaType areaType = reader.GetAreaTypes(new List<int> { AreaTypeIds.Sha }).First();
            Assert.AreEqual(AreaTypeIds.Sha, areaType.Id);
            Assert.AreEqual("SHA", areaType.ShortName);
            Assert.AreEqual("Strategic Health Authority", areaType.Name);

            // No area types
            Assert.AreEqual(0, reader.GetAreaTypes(new List<int>()).Count);
        }

        [TestMethod]
        public void TestGetAreaWithAddressByAreaTypeId()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<AreaAddress> areas = reader.GetAreaWithAddressByAreaTypeId(AreaTypeIds.GpPractice);
            Assert.IsTrue(areas.Count > 6000, "Less than expected areas: " + areas.Count);
            Assert.AreEqual("TS10 4NW", areas.First(x => x.Code == "A81018").Postcode);
        }

        [TestMethod]
        public void TestGetChildAreasInRegionByIndicatorIds()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<IArea> areas = reader
                .GetChildAreas(AreaCodes.Sha_EastOfEngland, AreaTypeIds.Pct);
            Assert.IsTrue(areas.Count > 0);

            // Luton included
            Assert.IsTrue(areas.Any(x => x.Code == AreaCodes.Pct_Luton));

            // Parent region is not included
            Assert.IsFalse(areas.Any(x => x.Code == AreaCodes.Sha_EastOfEngland));
        }

        [TestMethod]
        public void TestGetAreaCount()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            int count = reader.GetAreaCountForAreaType(AreaTypeIds.Pct);
            Assert.IsTrue(count > 100 && count < 200);

            count = reader.GetAreaCountForAreaType(AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.IsTrue(count > 100 && count < 200);

            count = reader.GetAreaCountForAreaType(AreaTypeIds.GpPractice);
            Assert.IsTrue(count > 6000 && count < 8000);
        }

        [TestMethod]
        public void TestGetAreaCodesForAreaType()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            IList<string> codes = reader.GetAreaCodesForAreaType(AreaTypeIds.GpPractice);

            Assert.AreEqual(reader.GetAreaCountForAreaType(AreaTypeIds.GpPractice), codes.Count);

            foreach (string code in codes)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(code));
            }
        }

        [TestMethod]
        public void TestGetChildAreaCodes()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            string code = AreaCodes.Pct_Suffolk;

            IList<string> childAreaCodes = reader.GetChildAreaCodes(code, AreaTypeIds.GpPractice);

            Assert.AreEqual(reader.GetChildAreaCount(code, AreaTypeIds.GpPractice), childAreaCodes.Count);

            foreach (string childCode in childAreaCodes)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(childCode));
            }
        }

        [TestMethod]
        public void TestGetChildAreaCount()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            int count = reader.GetChildAreaCount(AreaCodes.Sha_EastOfEngland, AreaTypeIds.Pct);
            Assert.IsTrue(count > 10 && count < 20);
            count = reader.GetChildAreaCount(AreaCodes.Pct_Sheffield, AreaTypeIds.GpPractice);
            Assert.IsTrue(count > 70 && count < 90);
        }

        [TestMethod]
        public void TestGetChildAreaCountForCategoryArea()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var area = new CategoryArea(AreaCodes.DeprivationDecile_Utla3);
            int count = reader.GetChildAreaCount(area, AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.IsTrue(count > 10 && count < 20);
        }

        [TestMethod]
        public void TestGetChildAreas()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<IArea> areas = reader.GetChildAreas(AreaCodes.Sha_EastOfEngland, AreaTypeIds.Pct);
            Assert.AreEqual(13, areas.Count);
            Assert.AreEqual(AreaCodes.Pct_Bedfordshire, areas[0].Code);
        }

        [TestMethod]
        public void TestGetParentAreas()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<Area> areas = reader.GetParentAreas("A81002");
            Assert.IsTrue(areas.Count >= 3);
            Assert.IsNotNull((from a in areas where a.Code == AreaCodes.Pct_StocktonOnTees select a).FirstOrDefault());
        }

        [TestMethod]
        public void TestGetParentsFromChildAreaIdAndParentAreaTypeId()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            // Area code and child area type id mismatch
            IList<string> areas = reader.GetParentCodesFromChildAreaId(AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.IsTrue(areas.Count > 50);
            Assert.IsTrue(areas.Contains(AreaCodes.Gor_London));

            // Area code and child area type id mismatch
            areas = reader.GetParentCodesFromChildAreaId(AreaTypeIds.Pct);
            Assert.IsTrue(areas.Count > 8 && areas.Count < 20);
            Assert.IsTrue(areas.Contains(AreaCodes.Sha_EastOfEngland));
        }

        [TestMethod]
        public void TestGetAreaUrl()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            // Links are available
            Assert.IsTrue(reader.GetAreaUrl(AreaCodes.CountyUa_CityOfLondon).StartsWith("http:"));
            Assert.IsTrue(reader.GetAreaUrl(AreaCodes.CountyUa_Buckinghamshire).StartsWith("http:"));

            // No links
            Assert.IsNull(reader.GetAreaUrl(AreaCodes.Pct_Norfolk));
            Assert.IsNull(reader.GetAreaUrl(AreaCodes.England));
        }

        [TestMethod]
        public void TestGetCategory()
        {
            var categoryTypeId = CategoryTypeIds.DeprivationDecileCountyAndUA2010;
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var category = reader.GetCategory(categoryTypeId, 5);
            Assert.AreEqual(5, category.Id);
            Assert.AreEqual(categoryTypeId, category.CategoryTypeId);
        }

        [TestMethod]
        public void TestGetCategories()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            var categoryIds = reader.GetCategories(CategoryTypeIds.DeprivationDecileCountyAndUA2010);
            Assert.AreEqual(10, categoryIds.Count);

            // Check are in ascending order
            Assert.AreEqual(1, categoryIds.First().Id);
            Assert.AreEqual(10, categoryIds.Last().Id);
        }

        [TestMethod]
        public void TestGetCategorisedAreas()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            int categoryId = reader.GetCategorisedArea(AreaCodes.Gp_Addingham, AreaTypeIds.Country,
                AreaTypeIds.GpPractice, CategoryTypeIds.DeprivationDecileGp2010).CategoryId;

            Assert.AreEqual(10, categoryId);
        }

        [TestMethod]
        public void TestGetCategorisedAreasForOneCategory()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<CategorisedArea> categories = reader.GetCategorisedAreasForOneCategory(AreaTypeIds.Country,
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, CategoryTypeIds.DeprivationDecileCountyAndUA2010,
                7);

            Assert.IsTrue(categories.Count > 0 && categories.Count < 20/*more than one tenth of 150*/);
        }

        [TestMethod]
        public void TestGetCategorisedAreasForAllCategories()
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();
            IList<CategorisedArea> categories = reader.GetCategorisedAreasForAllCategories(AreaTypeIds.Country,
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, CategoryTypeIds.DeprivationDecileCountyAndUA2010);

            // Expect ~150 areas
            Assert.IsTrue(categories.Count > 100 && categories.Count < 200);
        }

        [TestMethod]
        public void TestGetCategoryType()
        {
            var id = CategoryTypeIds.DeprivationDecileCountyAndUA2010;
            var categoryType = Reader().GetCategoryType(id);
            Assert.AreEqual(id, categoryType.Id);
        }

        [TestMethod]
        public void TestGetAllCategoryTypes()
        {
            var categoryTypes = Reader().GetAllCategoryTypes();
            Assert.IsTrue(categoryTypes.Select(x => x.Id).Contains(CategoryTypeIds.EthnicGroups5));
        }

        [TestMethod]
        public void TestCategoryTypeAlsoLoadsCategories()
        {
            var id = CategoryTypeIds.DeprivationDecileCountyAndUA2010;
            var categoryType = Reader().GetCategoryType(id);
            Assert.IsNotNull(categoryType.Categories);
            Assert.AreEqual(10, categoryType.Categories.Count);
        }

        [TestMethod]
        public void TestGetCategoryTypesFromIds()
        {
            var categoryTypes = Reader().GetCategoryTypes(new List<int>
            {
                CategoryTypeIds.EthnicGroups5,
                CategoryTypeIds.LsoaDeprivationDecilesWithinArea2010
            });

            Assert.AreEqual(2, categoryTypes.Count);
            Assert.AreEqual(1, categoryTypes.Count(x => x.Id == CategoryTypeIds.EthnicGroups5));
            Assert.AreEqual(1, categoryTypes.Count(x => x.Id == CategoryTypeIds.LsoaDeprivationDecilesWithinArea2010));
        }

        /// <summary>
        /// Regression test to ensure CCG names are formatted correctly
        /// </summary>
        [TestMethod]
        public void TestNhsIsUppercaseForAllCcgs()
        {
            List<IArea> areasWithNhsNotUppercase = new List<IArea>();

            var ccgs = ReaderFactory.GetAreasReader().GetAreasByAreaTypeId(AreaTypeIds.CcgsPostApr2019);
            foreach (var ccg in ccgs)
            {
                if (ccg.Name.Contains("Nhs") || ccg.ShortName.Contains("Nhs"))
                {
                    areasWithNhsNotUppercase.Add(ccg);
                    Console.WriteLine(ccg.Name + "  " + ccg.ShortName);
                }
            }

            Assert.AreEqual(0, areasWithNhsNotUppercase.Count,
                "The names of some CCGs contain 'Nhs' instead of 'NHS', see console output");
        }

        [TestMethod]
        public void TestGetSupportedAreaTypes()
        {
            var areaTypes = ReaderFactory.GetAreasReader().GetSupportedAreaTypes();
            Assert.IsTrue(areaTypes.Any());
        }

        [TestMethod]
        public void TestNearbyPractices()
        {
            var practices = ReaderFactory.GetAreasReader()
                .GetNearbyAreas("352107", "529262", AreaTypeIds.GpPractice);
            Assert.IsTrue(practices.Count > 0);
            var practice = new NearByAreas
            {
                AreaName = "BIRBECK MEDICAL GROUP",
                AreaCode = "A82035",
                AreaTypeID = "7",
                AddressLine1 = "PENRITH HEALTH CENTRE",
                AddressLine2 = "BRIDGE LANE",
                Postcode = "CA11 8HW",
                Easting = 352106.9941,
                Northing = 529262.0139
            };

            Assert.IsTrue(practices.Any(x => x.AreaName.ToLower() == practice.AreaName.ToLower()));
            Assert.IsTrue(practices.Any(x => x.AreaCode == practice.AreaCode));
            Assert.IsTrue(practices.Any(x => x.AreaTypeID == practice.AreaTypeID));
            Assert.IsTrue(practices.Any(x => x.Postcode == practice.Postcode));
            Assert.IsTrue(practices.Any(x => x.Easting == practice.Easting));
            Assert.IsTrue(practices.Any(x => x.Northing == practice.Northing));
        }

        [TestMethod]
        public void TestGetNearestNeighbours()
        {
            var nearestNeighbours = Reader().GetNearestNeighbours(AreaCodes.CountyUa_Cumbria, 1);

            Assert.IsTrue(nearestNeighbours.Count > 0);
        }

        private static IAreasReader Reader()
        {
            return ReaderFactory.GetAreasReader();
        }
    }
}