using System;
using System.Linq;
using FingertipsDataExtractionTool.AverageCalculator;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.PholioObjects;
using Unity;

namespace FingertipsDataExtractionTool
{
    public interface IProgram
    {
        void Go(string[] args);
    }

    class Program : IProgram
    {
        private ILogger _logger;
        private IFileDeleter _fileDeleter;
        private IDataFileGenerator _dataFileGenerator;
        private IPracticePopulationFileGenerator _practicePopulationFileGenerator;
        private IBulkCoreDataSetAverageCalculator _bulkCoreDataSetAverageCalculator;

        public Program(ILogger logger, IFileDeleter fileDeleter,
            IDataFileGenerator dataFileGenerator, IPracticePopulationFileGenerator practicePopulationFileGenerator,
            IBulkCoreDataSetAverageCalculator bulkCoreDataSetAverageCalculator)
        {
            _logger = logger;

            try
            {
                _fileDeleter = fileDeleter;
                _dataFileGenerator = dataFileGenerator;
                _practicePopulationFileGenerator = practicePopulationFileGenerator;
                _bulkCoreDataSetAverageCalculator = bulkCoreDataSetAverageCalculator;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                // Check command line arguments
                if (args.Length == 0)
                {
                    throw new FingertipsException("Command line argument either excel or coredataset must be specified");
                    //                    args = new string[] { "coredataset" }; // Use this line to debug in VS
                }

                var program = (IProgram)UnityContainerProvider.GetUnityContainer().Resolve<Program>();
                program.Go(args);
            }
            catch (Exception ex)
            {
                LogExceptionToConsole(ex);
                ExceptionLog.LogException(ex, null);
            }
        }

        public void Go(string[] args)
        {
            Init();
            Menu(args);
            Run(args);
        }

        public void Run(string[] args)
        {
            var firstOption = args.Any()
                ? args[0]
                : Console.ReadKey().KeyChar.ToString();

            switch (firstOption)
            {
                case "1":
                case "excel":
                    GenerateCsvFiles();
                    break;
                case "2":
                case "coredataset":
                    CalculateCoreDataSetAverages();
                    break;
                default:
                    Main(null);
                    break;
            }
        }

        public void Init()
        {
            Console.Title = "Fingertips Data Extraction Tool";
        }

        private void Menu(string[] args)
        {
            if (args == null || args.Any()) return;

            _logger.Info("---FDET STARTED---");
            Console.Clear();
            Console.WriteLine("Please select from the following options");
            Console.WriteLine("1- Generate CSV files");
            Console.WriteLine("2- Calculate CoreDataSet Averages");
            Console.WriteLine("3- Exit");
        }

        private void GenerateCsvFiles()
        {
            try
            {
                _logger.Info("--- Starting generation of CSV files ---");
                _fileDeleter.DeleteAllExistingFiles();
                _dataFileGenerator.Generate();
                _practicePopulationFileGenerator.Generate();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void CalculateCoreDataSetAverages()
        {
            try
            {
                _logger.Info("--- Starting calculation of CoreDataSet averages ---");
                var groupings = new CalculatorGroupingListProvider(ReaderFactory.GetGroupDataReader()).GetGroupings();
                _bulkCoreDataSetAverageCalculator.Calculate(groupings, new AverageCalculationConfig());
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            LogExceptionToConsole(ex);
            ExceptionLog.LogException(ex, null);
            NLogHelper.LogException(_logger, ex);
        }

        /// <summary>
        /// Log to console ver useful to help debug start up issues
        /// </summary>
        private static void LogExceptionToConsole(Exception ex)
        {
            WriteException(ex);
            if (ex.InnerException != null)
            {
                WriteException(ex.InnerException);
            }

            // Log to this file only way to see error if the application does not start
            // in task runner
            System.IO.File.AppendAllText(@"C:\fingertips-logs\fdet-error.txt",
                ex.Message + ex.StackTrace);
        }

        private static void WriteException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.GetType().FullName);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
