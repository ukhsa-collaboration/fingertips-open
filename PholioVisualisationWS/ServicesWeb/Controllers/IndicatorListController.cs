using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using PholioVisualisation.Services;
using PholioVisualisation.UserData.IndicatorLists;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// This controller contains actions to manage the user defined indicator lists
    /// </summary>
    [RoutePrefix("api")]
    public class IndicatorListController : BaseController
    {
        //TODO move services from InternalServicesController to here. For some reason the routes are not registered in this controller
    }
}