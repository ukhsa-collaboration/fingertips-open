using Ckan.Exceptions;
using Ckan.Model;
using Ckan.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckan.Client
{
    public class CkanApi : ICkanApi
    {
        private readonly ICkanHttpClient _ckanHttpClient;

        public CkanApi(ICkanHttpClient _ckanHttpClient)
        {
            this._ckanHttpClient = _ckanHttpClient;
        }

        public CkanGroup CreateGroup(CkanGroup ckanGroup)
        {
            return PostActionResult(ActionNames.GroupCreate, ckanGroup);
        }

        public CkanGroup UpdateGroup(CkanGroup ckanGroup)
        {
            return PostActionResult(ActionNames.GroupUpdate, ckanGroup);
        }

        public CkanPackage CreatePackage(CkanPackage package)
        {
            return PostActionResult(ActionNames.PackageCreate, package);
        }

        public CkanResource CreateResource(CkanResource resource)
        {
            var json = _ckanHttpClient.UploadResource(ActionNames.ResourceCreate, resource);
            var response = DeserialiseResponse<CkanResource>(json);
            return response.Result;
        }

        public CkanPackage UpdatePackage(CkanPackage ckanPackage)
        {
            return PostActionResult(ActionNames.PackageUpdate, ckanPackage);
        }

        public CkanGroup GetGroup(string id)
        {
            try
            {
                return GetActionResult<CkanGroup>(ActionNames.GroupShow,
                    GetIdParameter(id));
            }
            catch (CkanObjectNotFoundExpection)
            {
                return null;
            }
        }

        public CkanPackage GetPackage(string id)
        {
            try
            {
                return GetActionResult<CkanPackage>(ActionNames.PackageShow,
                    GetIdParameter(id));
            }
            catch (CkanObjectNotFoundExpection)
            {
                return null;
            }
        }

        public CkanOrganisation GetOrganization(string id)
        {
            return GetActionResult<CkanOrganisation>(ActionNames.OrganizationShow,
                GetIdParameter(id));
        }

        private static Dictionary<string, string> GetIdParameter(string id)
        {
            var parameters = new Dictionary<string, string>
            {
                {"id", id}
            };
            return parameters;
        }

        public List<string> GetGroupIds()
        {
            return GetStringListActionResult(ActionNames.GroupList);
        }

        private List<string> GetStringListActionResult(string actionName)
        {
            return GetActionResult<string[]>(actionName, new Dictionary<string, string>())
                .ToList();
        }

        private T GetActionResult<T>(string actionName,
            Dictionary<string, string> parameters)
        {
            // Log get request
            Console.WriteLine("#GET:" + actionName);

            // Do get and parse response
            var json = _ckanHttpClient.GetAction(actionName, parameters);
            var response = DeserialiseResponse<T>(json);
            return response.Result;
        }

        private T PostActionResult<T>(string actionName, T bodyObject)
        {
            // Log post action and body
            Console.WriteLine("#POST BODY:" + actionName);
            var bodyAsJson = JsonConvert.SerializeObject(bodyObject);
            Console.WriteLine(bodyAsJson);

            // Do post and parse response
            var json = _ckanHttpClient.PostAction(actionName, bodyObject);
            var response = DeserialiseResponse<T>(json);
            return response.Result;
        }

        private static CkanResultResponse<T> DeserialiseResponse<T>(string json)
        {
            Console.WriteLine(json);
            CkanErrorFinder.CheckResponseForError(json);
            var response = JsonConvert.DeserializeObject<CkanResultResponse<T>>(json);

            if (response.Success == false)
            {
                throw new CkanApiException("#EXCEPTION: Response not successful");
            }

            return response;
        }
    }
}
