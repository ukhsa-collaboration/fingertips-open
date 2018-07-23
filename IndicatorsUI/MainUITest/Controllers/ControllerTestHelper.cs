using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUITest.Controllers
{
    public class ControllerTestHelper
    {
        public static void AssertRedirectAction(RedirectToRouteResult result,
            string action, string controller)
        {
            Assert.AreEqual(action, result.RouteValues["action"].ToString());
            Assert.AreEqual(controller, result.RouteValues["controller"].ToString());
        }

        public static void AssertRedirectAction(RedirectToRouteResult result,
            string action)
        {
            Assert.AreEqual(action, result.RouteValues["action"].ToString());
        }

        public static IEnumerable<string> GetModelStateErrors(ViewResult result)
        {
            var errors = result.ViewData.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            return errors;
        }

        public static void AssertModelStateIsNotValid(ViewResult result)
        {
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
        }
    }
}