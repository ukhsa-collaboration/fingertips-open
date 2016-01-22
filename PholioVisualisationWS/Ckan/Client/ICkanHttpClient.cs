using System.Collections.Generic;
using Ckan.Model;

namespace Ckan.Client
{
    public interface ICkanHttpClient
    {
        /// <summary>
        /// Sends a GET request to a remote CKAN repository.
        /// </summary>
        /// <param name="actionName">CKAN action name</param>
        /// <param name="parameters">Parameters appropriate to the action</param>
        /// <returns>JSON serialised response</returns>
        string GetAction(string actionName, Dictionary<string, string> parameters);

        /// <summary>
        /// Sends a POST request to a remote CKAN repository.
        /// </summary>
        /// <param name="actionName">CKAN action name</param>
        /// <param name="bodyObject">CKAN object that will be serialised to JSON and comprise the request body</param>
        /// <returns>JSON serialised response</returns>
        string PostAction(string actionName, object bodyObject);

        /// <summary>
        /// Uploads a resource to a remote CKAN repository.
        /// </summary>
        /// <param name="actionName">CKAN action name</param>
        /// <param name="resource">The resource to be uploaded</param>
        /// <returns>JSON serialised response</returns>
        string UploadResource(string actionName, CkanResource resource);
    }
}