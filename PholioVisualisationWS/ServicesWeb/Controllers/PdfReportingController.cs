using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Web.Http;
using NHibernate.Type;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using ServicesWeb.Common;

namespace ServicesWeb.Controllers
{
    /// <summary>
    /// Services to support generation of PDF reports for specific areas.
    /// </summary>
    [RoutePrefix("api")]
    public class PdfReportingController : BaseController
    {
        /// <summary>
        ///     Gets spine chart stats for every indicator of a profile organised into domains.
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_codes">List of area codes</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="include_recent_trends">Whether or not to include recent trends [default = "no"]</param>
        [HttpGet]
        [Route("pdf/spine_chart")]
        [AspNetCacheProfile("PdfService")]
        public IList<SpineChartTableData> SpineChartData(int profile_id = 0,
            string area_codes = null, int child_area_type_id = 0, string include_recent_trends = "no")
        {
            try
            {
                var parameters = new SpineChartDataParameters
                {
                    ProfileId = profile_id,
                    ChildAreaTypeId = child_area_type_id,
                    AreaCodes = new StringListParser(area_codes).StringList,
                    BenchmarkAreaCodes = new List<string> { AreaCodes.England},
                    IncludeRecentTrends = ServiceHelper.ParseYesOrNo(include_recent_trends)
                };

                return new SpineChartDataAction().GetResponse(parameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets the benchmark area, parent area and child areas.
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="parent_area">Parent area code</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        [HttpGet]
        [Route("pdf/child_areas")]
        [AspNetCacheProfile("PdfService")]
        public ChildAreasForPdfs ChildAreas(int profile_id = 0, string parent_area = null,
            int child_area_type_id = 0)
        {
            try
            {
                return new ChildAreasForPdfsAction().GetResponse(profile_id,
                    parent_area, child_area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets each domain of a profile with the metadata the area data for each indicator.
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="benchmark_area_codes">List of benchmark area codes</param>
        [HttpGet]
        [Route("pdf/national_values")]
        [AspNetCacheProfile("PdfService")]
        public IList<DomainNationalValues> NationalValues(int profile_id = 0,
            int child_area_type_id = 0, string benchmark_area_codes = null)
        {
            try
            {
                return new NationalValuesForPdfsAction().GetResponse(profile_id, child_area_type_id,
                    new StringListParser(benchmark_area_codes).StringList);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets supporting information specific to an area.
        /// </summary>
        /// <remarks>
        /// Only available for certain profiles, e.g. Health Profiles
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="area_code">Area code</param>
        [HttpGet]
        [Route("pdf/supporting_information")]
        [AspNetCacheProfile("PdfService")]
        public PdfSupportingInformation SupportingInformation(int profile_id = 0, string area_code = null)
        {
            try
            {
                return new SupportingInformationForPdfsAction().GetSupportingInformation(profile_id, area_code);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

    }
}