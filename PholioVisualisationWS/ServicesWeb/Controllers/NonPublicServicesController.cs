using System;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ServiceActions;

namespace ServicesWeb.Controllers
{
    /// <summary>
    /// Services that are used by Fingertips or Longer Lives but should not be included
    /// in the public API.
    /// </summary>
    [RoutePrefix("api")]
    public class NonPublicServicesController : BaseController
    {
        /// <summary>
        /// Gets an NHS Choices ID for a given area
        /// </summary>
        /// <param name="area_code">Area code</param>
        [HttpGet]
        [Route("area/nhs_choices_area_id")]
        public string GetNhsChoicesAreaId(string area_code)
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetNhsChoicesAreaId(area_code);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a Chimat resource ID for a given area and profile
        /// </summary>
        /// <param name="area_code">Area code</param>
        /// <param name="profile_id">Profile ID</param>
        [HttpGet]
        [Route("area/chimat_resource_id")]
        public int GetChimatResourceId(string area_code, int profile_id)
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetChimatResourceId(area_code, profile_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets Longer Lives area details for a specific profile
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="group_id">Group ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="area_code">Area code</param>
        [HttpGet]
        [Route("area_details")]
        public object GetAreaDetails(int profile_id = 0, int group_id = 0,
            int child_area_type_id = 0, string area_code = null)
        {
            try
            {
                return new AreaDetailsAction().GetResponse(profile_id, group_id, child_area_type_id, area_code);
            }
            catch (Exception ex)
            {
                Log(ex);

                throw;
            }
        }
    }
}