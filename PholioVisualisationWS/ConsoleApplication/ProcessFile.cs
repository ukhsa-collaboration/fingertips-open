using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication;

namespace ConsoleApplication
{
    /// <summary>
    /// Takes an input file and produces an output file.
    /// Change the code in the foreach loop to do whatever you need.
    /// </summary>
    public class ProcessFile : IConsoleTask
    {
        public const string InputFileName = @"C:\temp\ui-profiles.txt";
        public const string OutputFileName = @"C:\temp\ui-profiles.sql";

        public void Do()
        {
            var lines = File.ReadAllLines(InputFileName);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                var bits = line.Split('\t');

                var id = bits[0].Trim();
                var groupIds = bits[1].Trim();

                newLines.Add("update ui_profiles set group_ids = '" +
                     groupIds + "' where id = " + id);
            }

            File.WriteAllLines(OutputFileName, newLines);
        }
    }
}
