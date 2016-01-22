using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication
{
    /// <summary>
    /// Changes all the files in a directory based on the 
    /// "from" and "to" names in the name mapping file.
    /// </summary>
    class FileNameChanger : IConsoleTask
    {
        private const string NameMappingFile = "filenames.csv";
        Dictionary<string, string> filemappings;
        
        public void Do()
        {
            Console.WriteLine("---START---");

            ReadNameMappingCsvFile();

            RenameFiles();

            Console.WriteLine("---DONE---");
            Console.ReadLine();
        }

        private void RenameFiles()
        {
            if (filemappings.Count > 0)
            {
                foreach (var m in filemappings)
                {
                    if (File.Exists(m.Key))
                    {
                        try
                        {
                            File.Move(m.Key, m.Value);
                        }
                        catch (IOException ex)
                        {

                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine(m.Key + " Does not Exists.");
                    }
                }
            }
        }


        private void ReadNameMappingCsvFile()
        {
           if (!File.Exists(NameMappingFile))
            {
                Console.WriteLine("Error: Name mapping CSV file is missing!");
            }
            else
            {
                filemappings = new Dictionary<string, string>();
                var lines = File.ReadAllLines(NameMappingFile);
                foreach (var line in lines)
                {
                    var record = line.Split(',');
                    filemappings.Add(record[0], record[1]);
                }
            }            
        }

    }
}
