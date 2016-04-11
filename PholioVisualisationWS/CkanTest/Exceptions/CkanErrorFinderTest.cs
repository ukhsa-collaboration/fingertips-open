using System;
using System.Collections.Generic;
using System.Linq;
using Ckan;
using Ckan.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.CkanTest.Exceptions
{
    [TestClass]
    public class CkanErrorFinderTest
    {
        [TestMethod]
        [ExpectedException(typeof(CkanObjectNotFoundExpection))]
        public void CheckResponseForError_Throws_Not_Found_Exception()
        {
            CheckResponse("package_show_not_found.json");
        }

        [TestMethod]
        [ExpectedException(typeof(CkanNotAuthorizedException))]
        public void CheckResponseForError_Throws_Not_Authorized_Exception()
        {
            CheckResponse("package_show_not_authorized.json");
        }

        [TestMethod]
        [ExpectedException(typeof(CkanApiException))]
        public void CheckResponseForError_Throws_Not_Authorized_Exception_For_Bad_Json()
        {
            CheckResponse("group_create_not_authorized.json");
        }

        [TestMethod]
        [ExpectedException(typeof(CkanTimeoutException))]
        public void CheckResponseForError_Throws_Timeout_Exception()
        {
            CheckResponse("resource_create_timeout.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(CkanInternalServerException))]
        public void CheckResponseForError_Throws_Internal_Server_Exception()
        {
            CheckResponse("resource_create_server_error.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(CkanApiException))]
        public void CheckResponseForError_Throws_Api_Exception_If_Unknown_Error()
        {
            CheckResponse("unknown_error.json");
        }

        public static void CheckResponse(string fileName)
        {
            CkanErrorFinder.CheckResponseForError(
                CkanTestHelper.GetExampleResponseFromFile(fileName));
        }
    }
}
