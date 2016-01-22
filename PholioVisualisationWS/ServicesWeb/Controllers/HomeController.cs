using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ServicesWeb.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route]
        public HttpResponseMessage Index()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("<h2>Fingertips API</h2>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
