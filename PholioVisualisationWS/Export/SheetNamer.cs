using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PholioVisualisation.Export
{
    /// <summary>
    /// Ensures all the sheets given valid names within an Excel file.
    /// </summary>
    public class SheetNamer
    {
        public const int MaximumLength = 30;

        private List<string> existingNames = new List<string>();
        private Regex specialCharactersRegex = new Regex(@"\[|\]|\*|/|:|\?|\\");

        /// <summary>
        /// Gets a sheet name that is unique for the current Excel file.
        /// </summary>
        public string GetSheetName(string profileName)
        {
            var name = TruncatedName(profileName);
            name = ReplaceSpecialCharacters(name);
            name = FormatDuplicateName(name);
            
            existingNames.Add(name);

            return name;
        }

        private string ReplaceSpecialCharacters(string name)
        {
            return specialCharactersRegex.Replace(name, " ");
        }

        private string FormatDuplicateName(string name)
        {
            int i = 2;
            var startName = name;
            while (DoesWorksheetNameAlreadyExist(name))
            {
                name = startName + " (" + i++ + ")";
            }
            return name;
        }

        private static string TruncatedName(string profileName)
        {
            string truncatedName = profileName;
            if (truncatedName.Length > MaximumLength - 7 /* to account for trailing '... (1)'*/)
            {
                truncatedName = truncatedName.Substring(0, MaximumLength - 7) + "...";
            }
            return truncatedName;
        }

        protected bool DoesWorksheetNameAlreadyExist(string name)
        {
            return existingNames.Where(x => x == name).Any();
        }


    }
}
