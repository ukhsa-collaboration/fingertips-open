using System.Web;
using System.Web.Routing;

namespace Fpm.MainUI.Helpers
{
    public static class RouteValueDictionaryExtensions
    {
        public static RouteValueDictionary AddQueryStringParameters(this RouteValueDictionary dict)
        {
            var querystring = HttpContext.Current.Request.QueryString;

            foreach (var key in querystring.AllKeys)
                if (!dict.ContainsKey(key))
                    dict.Add(key, querystring.GetValues(key)[0]);

            return dict;
        }

        public static RouteValueDictionary ExceptFor(this RouteValueDictionary dict, params string[] keysToRemove)
        {
            foreach (var key in keysToRemove)
                if (dict.ContainsKey(key))
                    dict.Remove(key);

            return dict;
        }
    }
}