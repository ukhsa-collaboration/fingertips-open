using System;
using System.Linq;
using DIResolver;
using FingertipsDataExtractionTool.AverageCalculator;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;

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
            _fileDeleter = fileDeleter;
            _dataFileGenerator = dataFileGenerator;
            _practicePopulationFileGenerator = practicePopulationFileGenerator;
            _bulkCoreDataSetAverageCalculator = bulkCoreDataSetAverageCalculator;
        }

        static void Main(string[] args)
        {
            try
            {
                IoC.Register();
                var program = IoC.Container.GetInstance<IProgram>();
                program.Go(args);
            }
            catch (Exception ex)
            {
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
                _fileDeleter.DeleteAllExistingFiles();
                _dataFileGenerator.Generate();
                _practicePopulationFileGenerator.Generate();
            }
            catch (Exception ex)
            {
                NLogHelper.LogException(_logger, ex);
                ExceptionLog.LogException(ex, null);
            }
        }

        private void CalculateCoreDataSetAverages()
        {
            try
            {
                var groupings = new CalculatorGroupingListProvider(ReaderFactory.GetGroupDataReader()).GetGroupings();
                _bulkCoreDataSetAverageCalculator.Calculate(groupings, new AverageCalculationConfig());
            }
            catch (Exception ex)
            {
                NLogHelper.LogException(_logger, ex);
                ExceptionLog.LogException(ex, null);
            }
        }
    }
}
