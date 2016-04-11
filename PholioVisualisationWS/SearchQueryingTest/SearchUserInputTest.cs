using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.SearchQueryingTest
{
    [TestClass]
    public class SearchUserInputTest
    {
        [TestMethod]
        public void TestValidText()
        {
            AssertOk(new SearchUserInput("baa"), "baa*");
            AssertOk(new SearchUserInput("baa caa"), "baa*", "caa*");
            AssertOk(new SearchUserInput("baa or caa"), "baa*", "caa*");
            AssertOk(new SearchUserInput("baa and caa"), "baa*", "caa*");
            AssertOk(new SearchUserInput("baa and caa daa"), "baa*", "caa*", "daa*");
            AssertOk(new SearchUserInput("baa caa and daa"), "baa*", "caa*", "daa*");
            AssertOk(new SearchUserInput("baa      caa     daa"), "baa*", "caa*", "daa*");
        }

        [TestMethod]
        public void TestSeparatorCharacters()
        {
            AssertQueryToTerms("bury st.edmunds", new[] { "bury*", "edmunds*", "st*" });
            AssertQueryToTerms("big,small", new[] { "big*", "small*" });
        }

        [TestMethod]
        public void TestSingleWordsQueriesMustContainAtLeast3Characters()
        {
            Assert.IsFalse(new SearchUserInput("a").IsQueryValid);
            Assert.IsFalse(new SearchUserInput("aa").IsQueryValid);
            Assert.IsTrue(new SearchUserInput("aaa").IsQueryValid);
        }

        [TestMethod]
        public void TestTwoCharacterPostcodeSearch()
        {
            Assert.IsTrue(new SearchUserInput("s1").IsQueryValid);
        }

        [TestMethod]
        public void TestTwoWordSearches()
        {
            // e.g. Great Yarmouth
            AssertQueryToTerms("great y", new[] { "great*", "y*" });
            AssertQueryToTerms("great ya", new[] { "great*", "ya*" });
            AssertQueryToTerms("great yar", new[] { "great*", "yar*" });
        }

        private static void AssertQueryToTerms(string searchText, params string[] termsExpected)
        {
            SearchUserInput searchUserInput = new SearchUserInput(searchText);

            var terms = searchUserInput.Terms;
            Assert.AreEqual(terms.Count, termsExpected.Length);

            foreach (var expectedTerm in termsExpected)
            {
                Assert.IsTrue(terms.Contains(expectedTerm));
            }
        }

        private static void AssertOk(SearchUserInput searchUserInput, params string[] expected)
        {
            Assert.IsTrue(searchUserInput.IsQueryValid);
            Assert.AreEqual(expected.Length, searchUserInput.Terms.Count);
            foreach (var s in expected)
            {
                Assert.IsTrue(searchUserInput.Terms.Contains(s));
            }
        }

        [TestMethod]
        public void TestContainsAnyNumbers()
        {
            Assert.IsTrue(new SearchUserInput("cb1").ContainsAnyNumbers);
            Assert.IsFalse(new SearchUserInput("cam").ContainsAnyNumbers);
        }

        [TestMethod]
        public void TestInvalidText()
        {
            AssertNotValid("");
            AssertNotValid(" ");
            AssertNotValid("  ");
            AssertNotValid(" "); // TAB
            AssertNotValid(null);
            AssertNotValid("(");
            AssertNotValid(")");
            AssertNotValid("=");
        }

        private static void AssertNotValid(string searchText)
        {
            var userSearchInput = new SearchUserInput(searchText);
            Assert.IsFalse(userSearchInput.IsQueryValid);
        }
    }
}
