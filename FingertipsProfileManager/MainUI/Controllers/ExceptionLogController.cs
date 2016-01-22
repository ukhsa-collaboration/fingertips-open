using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;

namespace Fpm.MainUI.Controllers
{
    public class ExceptionLogController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        public ActionResult Index()
        {
            var model = new ExceptionGridModel();
            model.ServerList = CommonUtilities.GetDistinctExceptionServers();

            GetAllExceptions(model);
            return View(model);
        }

        [Route("exceptionlog/{id}")]
        public ActionResult ExceptionDetails(int id)
        {
            var exception = ReaderFactory.GetProfilesReader().GetException(id);
            return View(exception);
        }

        public ActionResult ShowExceptionDetails(string exceptionId)
        {
            var exception = _reader.GetException(Convert.ToInt32(exceptionId));
            return PartialView("_ExceptionDetail", exception);
        }

        public ActionResult ReloadExceptions(int exceptionDays, string exceptionServer)
        {
            var model = new ExceptionGridModel();
            model.ServerList = CommonUtilities.GetDistinctExceptionServers();

            GetAllExceptions(model, Convert.ToInt32(exceptionDays), exceptionServer);
            return View("Index", model);
        }

        private void GetAllExceptions(ExceptionGridModel exceptions,
            int exceptionDays = 0, string exceptionServer = "")
        {
            if (string.IsNullOrEmpty(exceptionServer))
            {
                exceptionServer = Environment.MachineName;
            }

            exceptions.ExceptionGrid = exceptionServer == ExceptionOptions.AllServers
                ? _reader.GetExceptionsForAllServers(exceptionDays)
                : _reader.GetExceptionsByServer(exceptionDays, exceptionServer);
        }
    }
}
