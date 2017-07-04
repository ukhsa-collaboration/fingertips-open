using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.ExceptionLog;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("exceptions")]
    public class ExceptionLogController : Controller
    {
        private readonly ExceptionsRepository _exceptionLogRepository = new ExceptionsRepository();

        [HttpGet]
        [Route("")]
        public ActionResult ExceptionIndex(int exceptionDays = 0, string exceptionServer = null)
        {
            var viewModel = new ExceptionIndexViewModel();

            viewModel.ServerList = CommonUtilities.GetDistinctExceptionServers();
            GetAllExceptions(viewModel, exceptionDays, exceptionServer);
            return View(viewModel);
        }

        [Route("exception/{id}")]
        public ActionResult ExceptionDetails(int id)
        {
            var exception = _exceptionLogRepository.GetException(id);
            return View(exception);
        }

        [Route("exception-partial/{id}")]
        public ActionResult ShowExceptionDetails(string id)
        {
            var exception = _exceptionLogRepository.GetException(Convert.ToInt32(id));
            return PartialView("_ExceptionDetail", exception);
        }

        private void GetAllExceptions(ExceptionIndexViewModel exceptions,
            int exceptionDays, string exceptionServer)
        {
            if (string.IsNullOrEmpty(exceptionServer))
            {
                exceptionServer = Environment.MachineName;
            }

            exceptions.ExceptionList = exceptionServer == ExceptionOptions.AllServers
                ? _exceptionLogRepository.GetExceptionsForAllServers(exceptionDays)
                : _exceptionLogRepository.GetExceptionsByServer(exceptionDays, exceptionServer);
        }
    }
}
