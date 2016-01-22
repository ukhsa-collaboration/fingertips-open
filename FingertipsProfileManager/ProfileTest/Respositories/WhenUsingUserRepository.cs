using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingUserRepository
    {

        private UserRepository _userRepository;

        [TestInitialize]
        public void Init()
        {
            _userRepository = new UserRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _userRepository.Dispose();
        }

        [TestMethod]
        public void TestGetUserAudit_MostRecentFirst()
        {
            var audits = _userRepository.GetUserAudit(new List<int> { 51 /*Tim Winters*/})
                .ToList();
          
           MostRecentIsFirst(audits.First().Timestamp, audits.Last().Timestamp);
        }

        public static void MostRecentIsFirst(DateTime first, DateTime last)
        {
            Assert.AreEqual(1, first.CompareTo(last));
        }
    }
}
