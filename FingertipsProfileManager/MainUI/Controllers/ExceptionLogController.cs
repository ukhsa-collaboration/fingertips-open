using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.ExceptionLog;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Exceptions;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("exceptions")]
    public class ExceptionLogController : Controller
    {
        private readonly IExceptionsRepository _exceptionLogRepository;

        public ExceptionLogController(IExceptionsRepository exceptionLogRepository)
        {
            _exceptionLogRepository = exceptionLogRepository;
        }

        [HttpGet]
        [Route]
        public ActionResult ExceptionIndex()
        {
            var viewModel = new ExceptionIndexViewModel()
            {
                ServerList = CommonUtilities.GetDistinctExceptionServers(),
                ExceptionList = GetExceptions(0, new string[] { ExceptionOptions.AllServers }),
                Server = ExceptionOptions.AllServers,
                LiveServersFilterApplied = false,
                ApiErrorsFilterApplied = false,
                NumberOfDays = 0
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("filtered-exceptions")]
        public ActionResult FilteredExceptions(ExceptionIndexViewModel viewModel)
        {
            IList<ExceptionLog> exceptionList;

            if (viewModel.LiveServersFilterApplied)
            {
                viewModel.ServerList = CommonUtilities.GetLiveExceptionServers();
                
                if (viewModel.Server == ExceptionOptions.AllServers)
                {
                    var servers = new[] { ServerNames.Live1, ServerNames.Live2 };
                    exceptionList = GetExceptions(viewModel.NumberOfDays, servers);
                }
                else
                {
                    exceptionList = GetExceptions(viewModel.NumberOfDays, new string[] { viewModel.Server });
                }
            }
            else
            {
                viewModel.ServerList = CommonUtilities.GetDistinctExceptionServers();

                exceptionList = GetExceptions(viewModel.NumberOfDays, new string[] { viewModel.Server });
            }

            if (viewModel.ApiErrorsFilterApplied)
            {
                exceptionList = exceptionList.Where(x => x.Url.Contains("api/")).ToList();
            }

            viewModel.ExceptionList = exceptionList;

            return View("ExceptionIndex", viewModel);
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

        private IList<ExceptionLog> GetExceptions(int exceptionDays, string[] exceptionServer)
        {
            if (exceptionServer == null)
            {
                exceptionServer = new string[] {Environment.MachineName};
            }

            return exceptionServer[0] == ExceptionOptions.AllServers
                ? _exceptionLogRepository.GetExceptionsForAllServers(exceptionDays)
                : _exceptionLogRepository.GetExceptionsForSpecificServers(exceptionDays, exceptionServer);
        }
    }
}
