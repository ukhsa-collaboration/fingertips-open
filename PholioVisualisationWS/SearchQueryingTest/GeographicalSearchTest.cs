using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.SearchQueryingTest
{
    [TestClass]
    public class GeographicalSearchTest
    {
        private const string NameOfCity = "Peterborough";

        [TestMethod]
        public void TestNoResultsForNonsenseInput()
        {
            Assert.AreEqual(0, GetResults("xhoawecapqfhasfdg").Count);
        }

        [TestMethod]
        public void TestNumberOfResultsDoNotExceedMaximumLimit()
        {
            Assert.IsTrue(GetResults("lon").Count == SearchEngine.ShortResultCount);
        }

        [TestMethod]
        public void TestWeightByPlaceType()
        {
            var results = GetResults("barrow");

            // Towns first
            Assert.AreEqual("Barrowford", results[0].PlaceName);
            Assert.AreEqual("Barrow-In-Furness", results[1].PlaceName);

            // Stoke
            results = SearchAndIncludePolygonArea("stoke", AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.AreEqual("Stoke-on-Trent", results[0].PlaceName);
        }

        [TestMethod]
        public void TestFirstPartOfPostcodeWithSingleDigit()
        {
            var results = GetResults("s1");
            Assert.AreEqual("s1", results.First().PlaceName.Split(' ').First().ToLower(), "Results should only contain s1");
        }

        [TestMethod]
        public void TestFullPostcodeWithFirstPartSingleDigit()
        {
            AssertMatchSingleResult("s1 2hh", GetResults("s1 2hh"));
        }

        [TestMethod]
        public void TestFullPostcodeWithFirstPartTwoDigits()
        {
            AssertMatchSingleResult("s19 6la", GetResults("s19 6la"));
        }

        [TestMethod]
        public void TestFullPostcodeWithoutLastLetter()
        {
            var results = GetResults("cb2 1n");
            Assert.AreEqual(10, results.Count);
            Assert.AreEqual("cb2 1na", results.First().PlaceName.ToLower());
        }

        [TestMethod]
        public void TestFullPostcodeWithInterveningSpaces()
        {
            var expected = "cb2 1na";
            AssertMatchSingleResult(expected, GetResults("cb2 1na"));
            AssertMatchSingleResult(expected, GetResults("cb2  1na"));
            AssertMatchSingleResult(expected, GetResults("cb2   1na"));
            AssertMatchSingleResult(expected, GetResults("cb2    1na"));
        }

        [TestMethod]
        public void TestFullPostcodeNoSpacesWithOneDigitInFirstHalf()
        {
            var expected = "cb2 1na";
            AssertMatchSingleResult(expected, GetResults("cb21na"));
        }

        [TestMethod]
        public void TestFullPostcodeNoSpacesWithTwoDigitsInFirstHalf()
        {
            var expected = "cb22 3aa";
            AssertMatchSingleResult(expected, GetResults("cb223aa"));
        }

        [TestMethod]
        public void TestFullPostcodeNoSpacesWithTwoDigitsInFirstHalfAndNoLettersYet()
        {
            var expected = "cb22 3aa";
            var results = GetResults("cb223");
            Assert.AreEqual(expected, results.First().PlaceName.ToLower());
        }

        private void AssertMatchSingleResult(string expected, IList<GeographicalSearchResult> results)
        {
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expected.ToLower(), results.First().PlaceName.ToLower(), "Only one postcode expected");
        }

        [TestMethod]
        public void TestPostcodeWithTwoDigitsAndSpaceAsExpected()
        {
            var results = GetResults("s19");
            Assert.AreEqual("s19", results.First().PlaceName.Split(' ').First().ToLower());
        }

        [TestMethod]
        public void TestCountyAssignedForTowns()
        {
            Assert.AreEqual("Cambridgeshire", GetResults("cambridge").First().County);
            Assert.AreEqual("Cumbria", GetResults("keswick").First().County);
            Assert.AreEqual("Torbay", GetResults("torquay").First().County);
        }

        [TestMethod]
        public void TestCountyNotAssignedForCounties()
        {
            Assert.AreEqual("", GetResults("cambridgeshire").First().County);
            Assert.AreEqual("", GetResults("cumbria").First().County);
            Assert.AreEqual("", GetResults("torbay").First().County);
        }

        [TestMethod]
        public void TestCountyComesAfterCitiesButBeforeTowns()
        {
            const string county = "Cambridgeshire";

            var results = GetResults("cam");

            // Assert: city is first
            Assert.AreEqual("Cambridge", results[0].PlaceName);

            // Assert: county is second or third (inconsistent position for unknown reason)
            Assert.IsTrue(results[1].PlaceName.Equals(county) || results[2].PlaceName.Equals(county));
        }

        [TestMethod]
        public void TestNamesWithSmallWords()
        {
            // in
            Assert.AreEqual("Barrow-In-Furness", GetResults("barrow-in-furness")[0].PlaceName);

            // on
            Assert.AreEqual("Walton-On-The-Naze", GetResults("walton-on-the-naze")[0].PlaceName);
        }

        [TestMethod]
        public void Test_District_Name_Is_Returned()
        {
            string name = "Erewash";
            var results = SearchAndIncludePolygonArea(name, AreaTypeIds.DistrictAndUnitaryAuthorityPreApr2019);
            Assert.AreEqual(name, results[0].PlaceName);
        }

        [TestMethod]
        public void TestOnlyParentAreasThatMatchRequestedParentAreaTypeIdAreFound()
        {
            // Test 1
            var results = SearchAndIncludePolygonArea("Peterborough", 
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019);
            Assert.AreEqual("Peterborough", results[0].PlaceName); // the city
            Assert.AreEqual("Peterborough", results[1].PlaceName); // the UA

            // Test 2
            results = SearchAndIncludePolygonArea("London", AreaTypeIds.PheCentresFrom2015);
            Assert.AreEqual("London", results[0].PlaceName);
            Assert.AreEqual("London PHE centre".ToLower(), results[1].PlaceName.ToLower());
        }

        [TestMethod]
        public void TestNamesAreFormattedAsInDatabase()
        {
            var results = SearchAndIncludePolygonArea("	London PHE", AreaTypeIds.PheCentresFrom2015);
            Assert.AreEqual("London PHE centre", results[0].PlaceName);
        }

        [TestMethod]
        public void TestParentAreaCodeAssignedMatchesParentAreaTypeId()
        {
            var parent1 = SearchAndIncludePolygonArea(NameOfCity, 
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019).First().PolygonAreaCode;

            var parent2 = SearchAndIncludePolygonArea(NameOfCity,
                AreaTypeIds.PheCentresFrom2015).First().PolygonAreaCode;

            Assert.AreNotEqual(parent1,parent2);
        }

        [TestMethod]
        public void TestEastingNorthingAreRetrievedForPlaceNameAndPostcode()
        {
            CheckEastingAndNorthingAreDefined(GetResults(NameOfCity).First());
            CheckEastingAndNorthingAreDefined(GetResults("WR10 2NR").First());
        }

        [TestMethod]
        public void TestEastingNorthingAreNotRetrievedForParentAreas()
        {
            CheckEastingAndNorthingAreNotDefined(GetResults("Cambridgeshire").First());
        }

        [TestMethod]
        public void TestEastingNorthingAreOnlyRetrievedOnRequest()
        {
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
            var parentAreasToInclude = new List<int> {areaTypeId};

            var results = new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = false
            }.SearchPlacePostcodes(NameOfCity, areaTypeId, parentAreasToInclude);
            CheckEastingAndNorthingAreNotDefined(results.First());

            results = new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = true
            }.SearchPlacePostcodes(NameOfCity, areaTypeId, parentAreasToInclude);
            CheckEastingAndNorthingAreDefined(results.First());
        }

        [TestMethod]
        public void Test_Parent_Areas_Are_Only_Retrieved_On_Request()
        {
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;

            // Include parent area
            var results = new GeographicalSearch().SearchPlacePostcodes(NameOfCity, areaTypeId, 
                new List<int> {areaTypeId});
            Assert.AreEqual(2, results.Count);

            // Do not include parent areas
            results = new GeographicalSearch().SearchPlacePostcodes(NameOfCity, areaTypeId,
                new List<int>());
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void TestPolygonAreaNameIsDefinedForPlaceAndPostcode()
        {
            CheckPolygonAreaNameIsDefined(GetResults(NameOfCity).First());
            CheckPolygonAreaNameIsDefined(GetResults("WR10 2NR").First());
        }

        [TestMethod]
        public void TestPolygonAreaNameIsDefinedForParentAreas()
        {
            CheckPolygonAreaNameIsDefined(GetResults("Cambridgeshire").First());
        }

        [TestMethod]
        public void TestRegionsCanBeSearched()
        {
            AssertAreaCanBeSearched(AreaTypeIds.GoRegion);
        }

//        [TestMethod]
//        public void TestSubRegionsCanBeSearched()
//        {
//            AssertAreaCanBeSearched(AreaTypeIds.Subregion);
//        }

        [TestMethod]
        public void TestPheCentres2013To2015CanBeSearched()
        {
            AssertAreaCanBeSearched(AreaTypeIds.PheCentresFrom2013To2015);
        }

        [TestMethod]
        public void TestPheCentres2015CanBeSearched()
        {
            AssertAreaCanBeSearched(AreaTypeIds.PheCentresFrom2015);
        }

        [TestMethod]
        public void TestNonSupportedAreasCannotBeSearched()
        {
            var results = SearchAndIncludePolygonArea("Peterborough", AreaTypeIds.Sha);
            Assert.AreEqual(0, results.Count);
        }

        private static List<GeographicalSearchResult> SearchAndIncludePolygonArea(string searchText, int areaTypeId)
        {
            return new GeographicalSearch().SearchPlacePostcodes(searchText, areaTypeId,
                new List<int> {areaTypeId});
        }

        private static void AssertAreaCanBeSearched(int areaTypeId)
        {
            if (IsAreaTypeSearchable(areaTypeId))
            {
                var results = SearchAndIncludePolygonArea("Peterborough", areaTypeId);
                Assert.AreNotEqual(0, results.Count);
            }
        }

        private static void CheckPolygonAreaNameIsDefined(GeographicalSearchResult result)
        {
            var areaName = result.PolygonAreaName;
            Assert.IsNotNull(areaName);
            Assert.AreNotEqual(0, areaName.Length);
        }

        private static void CheckEastingAndNorthingAreNotDefined(GeographicalSearchResult result)
        {
            Assert.AreEqual(0, result.Easting);
            Assert.AreEqual(0, result.Northing);
        }

        private static void CheckEastingAndNorthingAreDefined(GeographicalSearchResult result)
        {
            Assert.AreNotEqual(0, result.Easting);
            Assert.AreNotEqual(0, result.Northing);
        }

        private static List<GeographicalSearchResult> GetResults(string text)
        {
            var areaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019;
            var results = new GeographicalSearch().SearchPlacePostcodes(text, areaTypeId, new List<int> {areaTypeId});
            return results;
        }

        private static bool IsAreaTypeSearchable(int areaTypeId)
        {
            return ReaderFactory.GetAreasReader().GetAreaType(areaTypeId).IsSearchable;
        }

    }
}
