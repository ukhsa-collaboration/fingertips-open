using System;

namespace Ckan.Exceptions
{
    public class CkanApiException : Exception
    {
        public CkanApiException(string message) : base(message)
        {
            
        }
    }
}
