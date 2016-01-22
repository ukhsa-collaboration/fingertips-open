using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication
{
    public class ParsePortalLinks
    {
        private const string Separator = ";";

        public void Do()
        {
            var lines = File.ReadLines(@"C:\temp\portal2.txt");
            var blob = string.Concat(lines);
            var matches = Regex.Matches(blob, "<a([^h]+)href=\"([^\"]+)\"[^>]*>([^<]+)</a>");

            var categories = File.CreateText(@"C:\temp\categories.txt");
            var links = File.CreateText(@"C:\temp\links.txt");

            int categoryId = 0;

            foreach (Match match in matches)
            {
                var locked = match.Groups[1].Value.Contains("locked");
                var href = match.Groups[2].Value;
                var label = match.Groups[3].Value.Replace("&#39;", "'");

                var isCategory = href == "#";

                if (label.Contains(Separator) || href.Contains(Separator))
                {
                    throw new Exception("Separator found!!! in " + label);
                }

                label = label.Trim();
                href = href.Replace("%2F", "/");
                href = href.Replace("%3F", "?");
                href = href.Replace("%3D", "=");
                href = href.Replace("%23", "#");
                href = href.Replace("%20", "");
                href = href.TrimEnd('/');

                if (isCategory)
                {
                    categoryId++;
                    categories.WriteLine(categoryId + Separator + label);
                }
                else
                {
                    // Site
                    links.WriteLine(categoryId + Separator + href + Separator + label + Separator + (locked ? "1" : "0"));
                }
            }

            categories.Close();
            links.Close();
        }
    }
}
