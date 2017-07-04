using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class AgeSorter
    {
        private Regex unwantedSymbolRegex = new Regex("[<>=]");
        private Regex digitRegex = new Regex(@"\d");
        private double textOnlyAgeKey = 1000;

        public IList<Age> SortByAge(IList<Age> ages)
        {
            var ageNumberToAge = new Dictionary<double, Age>();

            foreach (var age in ages)
            {
                var ageNumber = GetAgeNumber(age);

                while (ageNumberToAge.ContainsKey(ageNumber))
                {
                    // The age number has already been used
                    ageNumber += 0.01;
                }

                ageNumberToAge.Add(ageNumber, age);
            }

            // Sort age numbers
            var ageNumbers = ageNumberToAge.Keys.ToList();
            ageNumbers.Sort();

            return ageNumbers.Select(ageNumber => ageNumberToAge[ageNumber]).ToList();
        }

        private double GetAgeNumber(Age age)
        {
            var digit = FindFirstDigit(age);
            return ParseAgeNumber(digit);
        }

        private string FindFirstDigit(Age age)
        {
            var parts = age.Name.Split('+', '-', ' ');
            string label = null;
            foreach (var part in parts)
            {
                if (digitRegex.IsMatch(part))
                {
                    label = unwantedSymbolRegex.Replace(part, "");
                    break;
                }
            }
            return label;
        }

        private double ParseAgeNumber(string label)
        {
            double ageNumber;
            if (double.TryParse(label, out ageNumber) == false)
            {
                ageNumber = textOnlyAgeKey++;
            }
            return ageNumber;
        }
    }
}