using System;
using System.Collections.Generic;
using PholioVisualisation.Parsers;

namespace PholioVisualisation.ServicesWeb.Helpers
{
    /// <summary>
    /// Class as a helper for service
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        /// Parse string yes or no to a boolean format
        /// </summary>
        /// <param name="yesOrNoString">Value yes or no</param>
        /// <param name="defaultBool">Default value if the string is empty</param>
        /// <returns>A boolean up is yes or no</returns>
        public static bool ParseYesOrNo(string yesOrNoString, bool defaultBool)
        {
            if (string.IsNullOrWhiteSpace(yesOrNoString))
            {
                return defaultBool;
            }

            yesOrNoString = yesOrNoString.ToLower();

            if (yesOrNoString.Equals("yes"))
            {
                return true;
            }

            if (yesOrNoString.Equals("no"))
            {
                return false;
            }

            return defaultBool;
        }

        /// <summary>
        /// Extract a string of elements separated by coma into a list of elements
        /// </summary>
        /// <param name="stringOfElements">String with areas codes separated by coma</param>
        /// <returns>List of string elements</returns>
        public static IList<string> StringListStringParser(string stringOfElements)
        {
            IList<string> listOfStringElements = null;

            if (stringOfElements != null)
            {
                listOfStringElements = new StringListParser(stringOfElements).StringList;
            }

            return listOfStringElements;
        }

        /// <summary>
        /// Extract a string of elements separated by coma into a array of elements
        /// </summary>
        /// <param name="stringOfElements">String with areas codes separated by coma</param>
        /// <returns>Array of string elements</returns>
        public static string[] StringArrayStringParser(string stringOfElements)
        {
            string[] listOfStringElements = null;

            if (stringOfElements != null)
            {
                listOfStringElements = new StringListParser(stringOfElements).StringArray;
            }

            return listOfStringElements;
        }


        /// <summary>
        /// Transform a string into a list of integer
        /// </summary>
        /// <param name="listString">string with integers separated with comas</param>
        /// <returns>List of integers parsed</returns>
        public static IList<int> IntListStringParser(string listString)
        {
            IList<int> listInt = null;
            if (!string.IsNullOrEmpty(listString))
            {
                listInt = new IntListStringParser(listString).IntList;
            }

            return listInt;
        }
    }
}