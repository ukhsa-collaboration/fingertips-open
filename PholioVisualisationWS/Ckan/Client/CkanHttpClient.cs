using Ckan.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Ckan.Client
{
    public class CkanHttpClient : ICkanHttpClient
    {
        /// <summary>
        /// The CKAN repository host name
        /// </summary>
        private string repository;

        /// <summary>
        /// The API key used to authenticate the HTTP requests as
        /// a specific CKAN user.
        /// </summary>
        private string apiKey;

        private string ApiResourcePath = "/api/3/action/";

        public CkanHttpClient(string repository, string apiKey)
        {
            this.repository = repository;
            this.apiKey = apiKey;

            /* Workaround for error message "Authentication failed because remote party has closed the transport stream"
            See: http://stackoverflow.com/questions/30664566/authentication-failed-because-remote-party-has-closed-the-transport-stream
            */
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | 
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public string GetAction(string actionName, Dictionary<string, string> parameters)
        {
            Task<string> task = ExecuteGetRequest(actionName, parameters);
            string json = task.Result;
            return json;
        }

        private async Task<string> ExecuteGetRequest(string actionName, Dictionary<string, string> parameters)
        {
            string result = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = GetBaseUrl();

                var requestUrl = ApiResourcePath + actionName + "?" + GetQueryParameters(parameters);
                Console.WriteLine("#URL: " + requestUrl);

                HttpResponseMessage response = await client.GetAsync(requestUrl);

                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        private static NameValueCollection GetQueryParameters(Dictionary<string, string> parameters)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                query.Add(parameter.Key, parameter.Value);
            }
            return query;
        }

        private Uri GetBaseUrl()
        {
            return new Uri(String.Format("{0}", repository));
        }

        public string PostAction(string actionName, object bodyObject)
        {
            Task<string> task = ExecutePostRequest(actionName, bodyObject);
            string json = task.Result;
            return json;
        }

        private async Task<string> ExecutePostRequest(string actionName, object bodyObject)
        {
            string result = null;
            using (var client = new HttpClient())
            {
                InitHttpClient(client);

                var json = JsonConvert.SerializeObject(bodyObject);
                var content = new StringContent(json);

                var requestUrl = ApiResourcePath + actionName;
                Console.WriteLine("#URL: " + requestUrl);
                HttpResponseMessage response = await client.PostAsync(requestUrl, content);

                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }

        public string UploadResource(string actionName, CkanResource resource)
        {
            Task<string> task = UploadData(actionName, resource);
            string json = task.Result;
            return json;
        }

        private async Task<string> UploadData(string actionName, CkanResource resource)
        {
            string result = null;
            using (var client = new HttpClient())
            {
                InitHttpClient(client);

                using (var content = new MultipartFormDataContent())
                {
                    var values = new[]
                    {
                        new KeyValuePair<string, string>("package_id", resource.PackageId),
                        new KeyValuePair<string, string>("format", resource.Format),
                        new KeyValuePair<string, string>("name", resource.Name),
                        new KeyValuePair<string, string>("description", resource.Description)
                    };

                    foreach (var keyValuePair in values)
                    {
                        content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                    }

                    var fileContent = new ByteArrayContent(resource.File.FileContents);
                    fileContent.Headers.Add("Content-Type", "application/vnd.ms-excel");
                    content.Add(fileContent, "upload", resource.File.FileName);

                    var requestUrl = ApiResourcePath + actionName;
                    Console.WriteLine("#URL: " + requestUrl);
                    HttpResponseMessage response = await client.PostAsync(requestUrl, content);

                    result = await response.Content.ReadAsStringAsync();
                }
            }

            return result;
        }

        private void InitHttpClient(HttpClient client)
        {
            client.BaseAddress = GetBaseUrl();
            client.DefaultRequestHeaders.Add("Authorization", apiKey);
            client.DefaultRequestHeaders.Add("X-CKAN-API-Key", apiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "PHE-CKAN-Uploader");
        }
    }
}