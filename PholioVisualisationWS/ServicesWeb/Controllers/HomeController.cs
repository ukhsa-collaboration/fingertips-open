using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    public class HomeController : ApiController
    {
        /// <summary>
        /// The front page of the Fingertips web services
        /// </summary>
        [HttpGet]
        [Route]
        public HttpResponseMessage Index()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(GetLandingPageContent());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private static string GetLandingPageContent()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<h2>Fingertips API</h2>");
            return sb.ToString();
        }
    }
}
