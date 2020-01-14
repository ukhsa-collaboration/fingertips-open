using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParameters
{
    public class BaseParameters
    {
        public const int UndefinedInteger = -1;
        public const double UndefinedDouble = -1;

        protected NameValueCollection Parameters;

        protected BaseParameters(NameValueCollection parameters)
        {
            Parameters = parameters;
        }

        public virtual bool AreValid
        {
            get { return true; }
        }

        protected static string DecodeHtmlString(string s)
        {
            if (HttpContext.Current != null)
            {
                // HttpContext is unavailable during unit testing
                return HttpContext.Current.Server.HtmlDecode(s);
            }
            return s;
        }

        protected bool ParseYesOrNo(string name, bool defaultValue)
        {
            string s = ParseString(name);
            if (string.IsNullOrWhiteSpace(s) == false)
            {
                s = s.ToLower();
                if (s == "yes")
                {
                    return true;
                }

                if (s == "no")
                {
                    return false;
                }
            }
            return defaultValue;
        }

        protected List<string> ParseStringList(string name, char divider)
        {
            List<string> list = new List<string>();
            string text = Parameters[name];
            if (string.IsNullOrEmpty(text) == false)
            {
                text = DecodeHtmlString(text);

                string[] bits = text.Split(divider);
                foreach (string bit in bits)
                {
                    string bitTrim = bit.Trim();
                    if (string.IsNullOrEmpty(bitTrim) == false)
                    {
                        list.Add(bitTrim);
                    }
                }
            }
            return list;
        }

        protected List<int> ParseIntList(string name, char divider)
        {
            List<int> list = new List<int>();
            string text = Parameters[name];
            if (string.IsNullOrEmpty(text) == false)
            {
                string[] bits = text.Split(divider);
                foreach (string bit in bits)
                {
                    int val;
                    if (int.TryParse(bit, out val))
                    {
                        list.Add(val);
                    }
                    // Parsing int fails for values greater than 2147483647.
                    // To keep it fail safe, return a number which is not in use.
                    // Also prevents later code throwing exceptions if no IDs in list
                    else
                    {
                        list.Add(UndefinedInteger);
                    }
                }
            }
            return list;
        }

        protected List<double> ParseDoubleList(string name, char divider)
        {
            List<double> list = new List<double>();
            string text = Parameters[name];
            if (string.IsNullOrEmpty(text) == false)
            {
                string[] bits = text.Split(divider);
                foreach (string bit in bits)
                {
                    double val;
                    if (double.TryParse(bit, out val))
                    {
                        list.Add(val);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Parses a string value. Default is "" if parameter is absent or invalid.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns></returns>
        protected string ParseString(string name)
        {
            string label = Parameters[name];
            if (string.IsNullOrEmpty(label) == false)
            {
                return label;
            }
            return string.Empty;
        }

        /// <summary>
        /// Parses a double value. Default is -1 if parameter is absent or invalid.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns></returns>
        protected double ParseDouble(string name)
        {
            string value = Parameters[name];
            if (string.IsNullOrEmpty(value) == false)
            {
                double val;
                if (double.TryParse(value, out val))
                {
                    return val;
                }
            }
            return UndefinedDouble;
        }

        /// <summary>
        /// Parses an int value. Default is -1 if parameter is absent or invalid.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns></returns>
        protected int ParseInt(string name)
        {
            string value = Parameters[name];
            if (string.IsNullOrEmpty(value) == false)
            {
                int val;
                if (int.TryParse(value, out val))
                {
                    return val;
                }
            }
            return UndefinedInteger;
        }

        /// <summary>
        /// Parses a boolean value. Default is false if parameter is absent or invalid.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns></returns>
        protected bool ParseBool(string name)
        {
            string value = Parameters[name];
            if (string.IsNullOrEmpty(value) == false)
            {
                bool val;
                if (bool.TryParse(value, out val))
                {
                    return val;
                }
            }
            return false;
        }

        protected Dictionary<int, string> ParseIntToStringDictionary(string name)
        {
            string value = Parameters[name];

            // e.g. "1:Q,4:Eng"
            Dictionary<int, string> map = new Dictionary<int, string>();
            if (string.IsNullOrEmpty(value) == false)
            {
                string[] bits = value.Split(',');
                foreach (string bit in bits)
                {
                    string[] components = bit.Split(':');
                    if (components.Length == 2)
                    {
                        string s2 = components[1].Trim();
                        int i1;
                        if (s2.Length > 0 && int.TryParse(components[0], out i1))
                        {
                            map.Add(i1, s2);
                        }
                    }
                }
            }
            return map;
        }
    }
}