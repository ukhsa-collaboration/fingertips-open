using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataConstructionTest
{
    /// <summary>
    /// Summary description for ProfileDetailsBuilderTest
    /// </summary>
    [TestClass]
    public class ProfileDetailsBuilderTest
    {
        [TestMethod]
        public void TestValidKey()
        {
            ProfileDetails details = new ProfileDetailsBuilder(
                ProfileUrlKeys.SevereMentalIllness).Build();

            Assert.AreEqual(ProfileIds.SevereMentalIllness, details.Id);
            Assert.IsNotNull(details.Title);
            Assert.AreEqual(ProfileUrlKeys.SevereMentalIllness, details.ProfileUrlKey);

            // Assert: domains are as expected
            Assert.AreEqual(5, details.Domains.Count);
            Assert.AreEqual(GroupIds.SevereMentalIllness_RiskFactors, 
                details.Domains[1].GroupId);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestInvalidKey()
        {
            new ProfileDetailsBuilder("not_exist").Build();
        }

        [TestMethod]
        public void TestParseStringList()
        {
            string[] expected = new[] { "a", "b", "c" };

            AssertExpected(ProfileDetailsBuilder.ParseStringList("a,b,c"), expected);

            // Trailing/leading spaces
            AssertExpected(ProfileDetailsBuilder.ParseStringList(" a , b , c "), expected);

            // Empty trailing value
            AssertExpected(ProfileDetailsBuilder.ParseStringList("a,b,c, "), expected);

            // Empty middle value
            AssertExpected(ProfileDetailsBuilder.ParseStringList("a,b,,c"), expected);

            // Empty leading value
            AssertExpected(ProfileDetailsBuilder.ParseStringList(",a,b,c"), expected);
        }

        [TestMethod]
        public void TestParseStringListReturnsEmptyListForNullString()
        {
            AssertEmptyList(null);
        }

        [TestMethod]
        public void TestParseStringListReturnsEmptyListForEmptyString()
        {
            AssertEmptyList(string.Empty);
        }

        [TestMethod]
        public void TestParseStringListReturnsEmptyListForWhitespaceString()
        {
            AssertEmptyList(" ");
        }

        private void AssertEmptyList(string input)
        {
            AssertExpected(ProfileDetailsBuilder.ParseStringList(input), new string[] { });
        }

        private void AssertExpected(IList<string> list, string[] expected)
        {
            Assert.AreEqual(expected.Length, list.Count());
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(list[i], expected[i]);
            }
        }
    }
}
