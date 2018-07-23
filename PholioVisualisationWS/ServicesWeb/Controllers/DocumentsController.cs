using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Management;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Services related to document
    /// </summary>
    [RoutePrefix("api")]
    public class DocumentsController : BaseController
    {
        /// <summary>
        /// Publish document
        /// </summary>
        [HttpPost]
        [Route("document")]
        public async Task<HttpResponseMessage> PublishDocument()
        {
            bool success;
            string liveUpdateKey = string.Empty;

            // Initialise the fpm document object
            FpmDocument document = new FpmDocument();

            try
            {
                // Initialise the multi part memory stream provider, read the request content
                // asynchonously into the memory stream provider
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // Loop through the memory stream provider contents
                foreach (var requestContents in provider.Contents)
                {
                    // Deserialise and read the document
                    if (requestContents.Headers.ContentDisposition.Name == "Document")
                    {
                        document = JsonConvert.DeserializeObject<FpmDocument>(requestContents.ReadAsStringAsync().Result);
                    }

                    // Deserialise and read the live update key
                    if (requestContents.Headers.ContentDisposition.Name == "LiveUpdateKey")
                    {
                        liveUpdateKey = JsonConvert.DeserializeObject<string>(requestContents.ReadAsStringAsync().Result);
                    }
                }

                // Make sure the document object is not null
                if (document != null)
                {
                    // The document must be published only if the live update key from
                    // the source system matches with the one in this web services project
                    if (liveUpdateKey == ApplicationConfiguration.Instance.GetLiveUpdateKey())
                    {
                        // Create an instance of the fpm document repository
                        FpmDocumentRepository repoFpmDocument = new FpmDocumentRepository();

                        // Publish document
                        repoFpmDocument.PublishDocument(document);

                        // Set the success to true
                        success = true;
                    }
                    else
                    {
                        // Set the success to false
                        success = false;
                    }
                }
                else
                {
                    // Set the success to false
                    success = false;
                }
            }
            catch (Exception)
            {
                // Error identified, set the success to false
                success = false;
            }

            // Set and return the http response message
            HttpResponseMessage result = Request.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, success);
            return result;
        }
    }
}