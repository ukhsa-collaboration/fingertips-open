using Ckan.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PholioVisualisation.CkanTest.Model
{
    [TestClass]
    public class CkanGroupTest
    {
        private const string JsonSubstringUsers = "\"users\":";

        [TestMethod]
        public void TestGetNewName()
        {
            Assert.AreEqual("phe-a", CkanGroup.GetNewName("a"));
        }

        [TestMethod]
        public void TestUsersIsSerializedWhenNotEmpty()
        {
            var group = new CkanGroup
            {
                Users = new List<CkanUser> { new CkanUser{Name="a"}}
            };

            var json = JsonConvert.SerializeObject(group);

            Assert.IsTrue(json.Contains(JsonSubstringUsers));
            Assert.IsTrue(json.Contains("\"name\":\"a\""));
        }

        [TestMethod]
        public void TestUsersIsNotSerializedWhenEmpty()
        {
            var group = new CkanGroup
            {
                Users = new List<CkanUser> { }
            };

            var json = JsonConvert.SerializeObject(group);

            Assert.IsFalse(json.Contains(JsonSubstringUsers));
        }

        [TestMethod]
        public void TestUsersIsNotSerializedWhenNull()
        {
            var group = new CkanGroup
            {
                Users = new List<CkanUser> { }
            };

            var json = JsonConvert.SerializeObject(group);

            Assert.IsFalse(json.Contains(JsonSubstringUsers));
        }
    }
}
