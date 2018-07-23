using System.Net;
using System.Net.Http;

namespace PholioVisualisation.ServicesWeb.Helpers
{
    /// <summary>
    /// Static http helper class
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// This method generates http response message
        /// </summary>
        /// <param name="success">Boolean value</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Http response message</returns>
        public static HttpResponseMessage GetHttpResponseMessage(HttpRequestMessage requestMessage, bool success, string errorMessage)
        {
            return requestMessage.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError,
                errorMessage);
        }
    }
}