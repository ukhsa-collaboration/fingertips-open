using Fpm.ProfileData.Entities.UserFeedback;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Fpm.ProfileDataTest.Repositories
{
    [TestClass]
    public class WhenUsingUserFeedbackRepository
    {
        private UserFeedbackRepository _repository;
        private int _newFeedbackId;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new UserFeedbackRepository();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_newFeedbackId > 0)
            {
                _repository.DeleteUserFeedback(_newFeedbackId);
            }
        }

        private UserFeedback NewUserFeedback()
        {
            return new UserFeedback
            {
                Url = "fingertips.phe.org.uk",
                WhatUserWasDoing = "testing the website",
                WhatWentWrong = "site never opens",
                Environment = "test",
                Timestamp = DateTime.Now
            };
        }

        [TestMethod]
        public void TestNewUserFeedback()
        {
            _newFeedbackId = _repository.NewUserFeedback(NewUserFeedback());
            Assert.IsNotNull(_newFeedbackId);
        }

        [TestMethod]
        public void TestGetAllUserFeedbacks()
        {
            var userFeedback1 = _repository.NewUserFeedback(NewUserFeedback());
            var userFeedback2 = _repository.NewUserFeedback(NewUserFeedback());
            var allFeedbacks = _repository.GetLatestUserFeedback(10);
            Assert.IsTrue(allFeedbacks.Any());

            _repository.DeleteUserFeedback(userFeedback1);
            _repository.DeleteUserFeedback(userFeedback2);
        }

        [TestMethod]
        public void TestGetFeedbackById()
        {
            _newFeedbackId = _repository.NewUserFeedback(NewUserFeedback());
            var feedback = _repository.GetFeedbackById(_newFeedbackId);
            Assert.AreEqual(_newFeedbackId, feedback.Id);
        }
    }
}