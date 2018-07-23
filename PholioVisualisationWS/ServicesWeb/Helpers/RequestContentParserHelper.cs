using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesWeb.Helpers
{
    /// <summary>
    /// Interface to define the request content parser helper
    /// </summary>
    public interface IRequestContentParserHelper
    {
        /// <summary>
        /// Read only string property to get the live update key from the application settings
        /// </summary>
        string LiveUpdateKey { get; }

        /// <summary>
        /// Method to deserialize the live update key, compare it against the live update key
        /// stored in the application settings and return the comparison result
        /// </summary>
        /// <param name="content">Http content</param>
        /// <returns>Comparison result as a boolean value</returns>
        bool DeserializeReadAndVerifyApiKey(HttpContent content);

        /// <summary>
        /// Asynchronous method to parse the http request message to indicator meta data text value objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of indicator meta dat text value objects</returns>
        Task<IList<IndicatorMetadataTextValue>> ParseMetadata(HttpRequestMessage requestMessage);

        /// <summary>
        /// Asynchronous method to parse the http request message to groupings objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of groupings objects</returns>
        Task<IList<Grouping>> ParseGroupings(HttpRequestMessage requestMessage);

        /// <summary>
        /// Method to parse the http request message to core data set objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of core data set objects</returns>
        IList<CoreDataSet> ParseCoreDataSets(HttpRequestMessage requestMessage);

        /// <summary>
        /// Asynchronous method to parse the http request message to profile id
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>ProfileId</returns>
        Task<int> ParseProfileId(HttpRequestMessage requestMessage);

        /// <summary>
        /// Asynchronous method to parse the http request to content item objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of content item objects</returns>
        Task<IList<ContentItem>> ParseContentItems(HttpRequestMessage requestMessage);

        /// <summary>
        /// Asynchronous method to parse the http request message to string
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>String of parsed content</returns>
        Task<string> ParseContent(HttpRequestMessage requestMessage);

        /// <summary>
        /// Method to parse large http request message to string
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>String of parsed content</returns>
        string ParseContentForLargeRequest(HttpRequestMessage requestMessage);
    }

    /// <summary>
    /// Contains the helper methods to parse the request content
    /// </summary>
    public class RequestContentParserHelper : IRequestContentParserHelper
    {
        /// <summary>
        /// This property returns the live update key stored in the application settings
        /// </summary>
        public string LiveUpdateKey
        {
            get
            {
                return ApplicationConfiguration.Instance.GetLiveUpdateKey();
            }
        }

        /// <summary>
        /// Asynchronous method to parse the http request message to indicator meta data text value objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of indicator meta dat text value objects</returns>
        public async Task<IList<IndicatorMetadataTextValue>> ParseMetadata(HttpRequestMessage requestMessage)
        {
            // Parse the request message
            string requestContent = await ParseContent(requestMessage);

            // Deserialize and return
            return JsonConvert.DeserializeObject<List<IndicatorMetadataTextValue>>(requestContent);
        }

        /// <summary>
        /// Asynchronous method to parse the http request message to groupings objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of groupings objects</returns>
        public async Task<IList<Grouping>> ParseGroupings(HttpRequestMessage requestMessage)
        {
            // Parse the request message
            string requestContent = await ParseContent(requestMessage);

            // Deserialize and return
            return JsonConvert.DeserializeObject<List<Grouping>>(requestContent);

        }

        /// <summary>
        /// Method to parse the http request message to core data set objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of core data set objects</returns>
        public IList<CoreDataSet> ParseCoreDataSets(HttpRequestMessage requestMessage)
        {
            // Parse the request message
            string requestContent = ParseContentForLargeRequest(requestMessage);

            // Deserialize and return
            return JsonConvert.DeserializeObject<List<CoreDataSet>>(requestContent);

        }

        /// <summary>
        /// Asynchronous method to parse the http request message to profile id
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>ProfileId</returns>
        public async Task<int> ParseProfileId(HttpRequestMessage requestMessage)
        {
            // Parse the request message
            string requestContent = await ParseContent(requestMessage);

            // Deserialize and return
            return JsonConvert.DeserializeObject<int>(requestContent);
        }

        /// <summary>
        /// Asynchronous method to parse the http request to content item objects
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>List of content item objects</returns>
        public async Task<IList<ContentItem>> ParseContentItems(HttpRequestMessage requestMessage)
        {
            // Parse the request message
            string requestContent = await ParseContent(requestMessage);

            // Deserialize and return
            return JsonConvert.DeserializeObject<List<ContentItem>>(requestContent);
        }

        /// <summary>
        /// Asynchronous method to parse the http request message to string
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>String of parsed content</returns>
        public async Task<string> ParseContent(HttpRequestMessage requestMessage)
        {
            string result = string.Empty;

            // Initialise the multi part memory stream provider, read the request content
            // asynchonously into the memory stream provider
            var provider = new MultipartMemoryStreamProvider();
            await requestMessage.Content.ReadAsMultipartAsync(provider);

            // Loop through the memory stream provider contents
            foreach (var requestContent in provider.Contents)
            {
                // Live update key
                if (requestContent.Headers.ContentDisposition.Name == "LiveUpdateKey")
                {
                    // Deserialize, read and verify live update key
                    if (!DeserializeReadAndVerifyApiKey(requestContent))
                    {
                        throw new Exception("Api key is not valid.");
                    }
                }
                // Result content
                else
                {
                    result = requestContent.ReadAsStringAsync().Result;
                }
            }

            // Return the result content
            return result;
        }

        /// <summary>
        /// Method to parse large http request message to string
        /// </summary>
        /// <param name="requestMessage">Http request message</param>
        /// <returns>String of parsed content</returns>
        public string ParseContentForLargeRequest(HttpRequestMessage requestMessage)
        {
            string result = string.Empty;

            // Initialise the multi part memory stream provider, read the request content
            // asynchonously into the memory stream provider. Large request for core data so
            // read differently to metadata and groupings.
            IEnumerable<HttpContent> parts = null;
            Task.Factory
                .StartNew(() => parts = requestMessage.Content.ReadAsMultipartAsync().Result.Contents,
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning, // guarantees separate thread
                    TaskScheduler.Default)
                .Wait();

            // Loop through the memory stream provider contents
            foreach (var requestContent in parts)
            {
                // Live update key
                if (requestContent.Headers.ContentDisposition.Name == "LiveUpdateKey")
                {
                    // Deserialize, read and verify live update key
                    if (!DeserializeReadAndVerifyApiKey(requestContent))
                    {
                        throw new Exception("Api key is not valid.");
                    }
                }
                // Result content
                else
                {
                    result = requestContent.ReadAsStringAsync().Result;
                }
            }

            // Return the result content
            return result;
        }

        /// <summary>
        /// The indicator metadata text values must be replaced only if the live update key from
        /// the source system matches with the one in this web services project
        /// </summary>
        public bool DeserializeReadAndVerifyApiKey(HttpContent content)
        {
            // Deserialize the live update key
            string liveUpdateKey = JsonConvert.DeserializeObject<String>(content.ReadAsStringAsync().Result);

            // Return the comparison result
            return !(string.IsNullOrWhiteSpace(liveUpdateKey) || liveUpdateKey != LiveUpdateKey);
        }
    }
}