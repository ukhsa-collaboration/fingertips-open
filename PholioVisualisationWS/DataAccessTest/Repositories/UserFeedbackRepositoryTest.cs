using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;
using System.Linq;

namespace PholioVisualisation.DataAccessTest.Repositories
{
    [TestClass]
    public class UserFeedbackRepositoryTest
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
                Url = "unittest.phe.org.uk",
                WhatUserWasDoing = "testing WhatUserWasDoing",
                WhatWentWrong = "testing WhatWentWrong",
                Email = "test@phe.org.uk",
                Environment = "test",
                Timestamp = DateTime.Now,
                HasBeenDealtWith = false
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

            var allFeedbacks = _repository.GetAllUserFeedbacks();
            Assert.IsTrue(allFeedbacks.Count() >= 2);

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
