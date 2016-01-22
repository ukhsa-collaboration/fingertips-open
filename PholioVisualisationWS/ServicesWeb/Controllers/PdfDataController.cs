using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServicesWeb.Controllers
{
    public class PdfDataController : BaseController
    {
        /// <summary>
        ///     Get the data required to produce a PDF spine chart for a profile.
        /// </summary>
        [HttpGet]
        [Route("data/pdf/spine_chart")]
        [AspNetCacheProfile("PdfService")]
        public IList<SpineChartTableData> SpineChartData(int profile_id = 0, 
            string area_codes = null,int child_area_type_id = 0)
        {
            try
            {
                return new SpineChartDataAction().GetResponse(profile_id,
                    child_area_type_id, new StringListParser(area_codes).StringList, AreaCodes.England);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets area information to support PDF generation.
        /// </summary>
        [HttpGet]
        [Route("data/pdf/child_areas")]
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
        ///     Gets national values to support PDF generation.
        /// </summary>
        [HttpGet]
        [Route("data/pdf/national_values")]
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
        ///     Gets data specific to an area to support PDF generation.
        /// </summary>
        [HttpGet]
        [Route("data/pdf/supporting_information")]
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