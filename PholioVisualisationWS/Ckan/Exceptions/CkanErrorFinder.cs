using System;
using System.Collections.Generic;
using System.Text;
using Ckan.Responses;
using Newtonsoft.Json;

namespace Ckan.Exceptions
{
    public class CkanErrorFinder
    {
        public static void CheckResponseForError(string json)
        {
            if (json.Contains("\"error\":"))
            {
                var ckanErrorResponse = JsonConvert.DeserializeObject<CkanErrorResponse>(json);
                var error = ckanErrorResponse.ErrorDictionary;

                ThrowExceptionIfObjectNotFound(error);

                ThrowCkanApiException(error);
            }

            ThrowExceptionIfInternalServerError(json);
            ThrowExceptionIfTimeoutError(json);
            ThrowExceptionIfBadJsonError(json);
        }

        private static void ThrowExceptionIfTimeoutError(string json)
        {
            if (json.Contains("504 Gateway Time-out"))
            {
                Console.WriteLine("#EXCEPTION: CkanTimeoutException");
                throw new CkanTimeoutException();
            }
        }

        private static void ThrowExceptionIfBadJsonError(string json)
        {
            if (json.Contains("JSON Error: No request body data"))
            {
                Console.WriteLine("#EXCEPTION: CkanApiException");
                throw new CkanApiException("Request was not accepted by the server, may be incorrect protocol: http/https");
            }
        }
        
        private static void ThrowExceptionIfInternalServerError(string json)
        {
            if (json.Contains("500 Internal Server Error"))
            {
                Console.WriteLine("#EXCEPTION: CkanInternalServerException");
                throw new CkanInternalServerException();
            }
        }

        private static void ThrowCkanApiException(Dictionary<string, object> error)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var errorInformation in error)
            {
                sb.AppendLine(errorInformation.Key + " : " + errorInformation.Value);
            }

            var exceptionMessage = sb.ToString();
            Console.WriteLine("#EXCEPTION: CkanApiException " + exceptionMessage);
            throw new CkanApiException(exceptionMessage);
        }

        private static void ThrowExceptionIfObjectNotFound(Dictionary<string, object> error)
        {
            const string typeKey = "__type"; 
            if (error.ContainsKey(typeKey))
            {
                var type = error[typeKey];
                if (type.Equals("Not Found Error"))
                {
                    Console.WriteLine("#EXCEPTION: " + type);
                    throw new CkanObjectNotFoundExpection();
                }

                if (type.Equals("Authorization Error"))
                {
                    //NOTE: Authorization Error will be returned if an object has been deleted
                    Console.WriteLine("#EXCEPTION: " + type + " (object may have been deleted)");
                    throw new CkanNotAuthorizedException();
                }

                throw new CkanApiException("Unknown type of error: " + typeKey);
            }
        }
    }
}