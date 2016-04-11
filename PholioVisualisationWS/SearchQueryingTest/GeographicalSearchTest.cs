using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            results = new GeographicalSearch().SearchPlacePostcodes("stoke", AreaTypeIds.CountyAndUnitaryAuthority);
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
            var results = GetResults("cam");
            int i = 0;
            Assert.AreEqual("Cambridge", results[i++].PlaceName);
            Assert.AreEqual("Camden", results[i++].PlaceName);
            Assert.AreEqual("Cambridgeshire", results[i++].PlaceName);
            Assert.AreEqual("Camberley", results[i++].PlaceName);
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
        public void TestOnlyParentAreasThatMatchRequestedParentAreaTypeIdAreFound()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", 
                AreaTypeIds.CountyAndUnitaryAuthority);
            Assert.AreEqual("Peterborough", results[0].PlaceName); // the city
            Assert.AreEqual("Peterborough", results[1].PlaceName); // the UA

            results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.Ccg);
            Assert.AreEqual("Peterborough", results[0].PlaceName);
            Assert.AreEqual("NHS Cambridgeshire and Peterborough CCG".ToLower(), results[1].PlaceName.ToLower());
        }

        [TestMethod]
        public void TestNamesAreFormattedAsInDatabase()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough CCG", AreaTypeIds.Ccg);
            Assert.AreEqual("NHS Cambridgeshire and Peterborough CCG", results[0].PlaceName);
        }

        [TestMethod]
        public void TestParentAreaCodeAssignedMatchesParentAreaTypeId()
        {
            var parent1 = new GeographicalSearch().SearchPlacePostcodes(NameOfCity, 
                AreaTypeIds.CountyAndUnitaryAuthority).First().PolygonAreaCode;
            var parent2 = new GeographicalSearch().SearchPlacePostcodes(NameOfCity,
                AreaTypeIds.Ccg).First().PolygonAreaCode;
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
            var results = new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = false
            }.SearchPlacePostcodes(NameOfCity, AreaTypeIds.CountyAndUnitaryAuthority);
            CheckEastingAndNorthingAreNotDefined(results.First());

            results = new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = true
            }.SearchPlacePostcodes(NameOfCity, AreaTypeIds.CountyAndUnitaryAuthority);
            CheckEastingAndNorthingAreDefined(results.First());
        }

        [TestMethod]
        public void TestCCGsAreOnlyRetrievedOnRequest()
        {
            var results = new GeographicalSearch
            {
                ExcludeCcGs = false
            }.SearchPlacePostcodes(NameOfCity, AreaTypeIds.Ccg);
            Assert.IsTrue(results.Count==2);

            results = new GeographicalSearch
            {
                ExcludeCcGs = true
            }.SearchPlacePostcodes(NameOfCity, AreaTypeIds.Ccg);
            Assert.IsTrue(results.Count==1);
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
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.GoRegion);
            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void TestSubRegionsCanBeSearched()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.Subregion);
            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void TestPheCentres2013To2015CanBeSearched()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.PheCentresFrom2013To2015);
            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void TestPheCentres2015CanBeSearched()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.PheCentresFrom2015);
            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException), "Area type id not supported for search: 5")]
        public void TestNonSupportedAreasCannotBeSearched()
        {
            var results = new GeographicalSearch().SearchPlacePostcodes("Peterborough", AreaTypeIds.Sha);
            Assert.AreEqual(0, results.Count);
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
            var results = new GeographicalSearch().SearchPlacePostcodes(text, AreaTypeIds.CountyAndUnitaryAuthority);
            return results;
        }
    }
}
