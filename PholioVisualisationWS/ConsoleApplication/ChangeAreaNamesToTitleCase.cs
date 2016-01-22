using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace ConsoleApplication
{
    /// <summary>
    /// When new areas are added to PHOLIO their names are usually in uppercase. For display 
    /// in Fingertips these names need to be converted to title case, e.g. "DURHAM" to "Durham".
    /// </summary>
    public class ChangeAreaNamesToTitleCase : IConsoleTask
    {
        private const char Separator = '\t';
        private const string OutputFile = @"C:\temp\areas.txt";

        /* Easiest way to import new names into SQL Server:
         
         1 Create table to hold imported data
CREATE TABLE [dbo].[newareas](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NULL,
	[short] [nvarchar](50) NULL
) ON [PRIMARY]

         2 Copy and paste file contents into SQL edit pane

         3 Update values from imported data
UPDATE t1
SET t1.[AreaName] = t2.name,
    t1.[AreaShortName] = t2.short
from [L_Areas] as t1
inner join [newareas] as t2
on t1.areacode = t2.code   

         */

        public void Do()
        {
            var areas = GetAreas();

            StringBuilder sb = new StringBuilder();
            foreach (var area in areas)
            {
                var name = FormatName(area.Name);
                ValidateName(name);
                var shortName = TryCreateShortNameIfNone(area.ShortName, name);
                shortName = FormatName(shortName);
                sb.AppendLine(area.Code + Separator + name + Separator + shortName);
            }

            File.WriteAllText(OutputFile, sb.ToString());
        }

        private static void ValidateName(string name)
        {
            if (name.Length > 255)
            {
                throw new FingertipsException("Name ('" + name + "') is greater than 255 characters");
            }
        }

        private static string TryCreateShortNameIfNone(string shortName, string name)
        {
            if (shortName == null)
            {
                shortName = name;
            }

            shortName = shortName.ToLower();

            shortName = RemoveSpecificTextFromShortName(shortName);

            if (shortName.Length > 50)
            {
                shortName = "TOO BIG !!!";
            }

            return shortName;
        }

        private static string RemoveSpecificTextFromShortName(string shortName)
        {
            var toRemove = new[] { "commissioning hub", "ccg", "nhs", "area", "team" };

            foreach (var subString in toRemove)
            {
                if (shortName.Contains(subString))
                {
                    shortName = shortName.Replace(subString, "");
                }
            }

            return shortName.Trim();
        }

        private static IList<Area> GetAreas()
        {
            IAreasReader areasReader = ReaderFactory.GetAreasReader();
            return areasReader.GetAreasByAreaTypeId(AreaTypeIds.Subregion);
        }

        private static string FormatName(string name)
        {
            if (name == null)
            {
                return "NULL";
            }

            CheckNameDoesNotContainSeparator(name);

            name = name.ToLower();
            name = new CultureInfo("en-GB", false).TextInfo.ToTitleCase(name);
            name = name
                .Replace("Ccg", "CCG")
                .Replace("Nhs", "NHS")
                .Replace("Qpp", "QPP")
                .Replace("And ", "and ");
            return name;
        }

        private static void CheckNameDoesNotContainSeparator(string name)
        {
            if (name.Contains(Separator))
            {
                throw new FingertipsException("Name ('" + name + "') contains '" + Separator + "'");
            }
        }
    }
}
