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
        private IExcelFileDeleter _excelFileDeleter;
        private IExcelFileGenerator _excelFileGenerator;
        private IPracticeProfilesExcelFileGenerator _practiceProfilesExcelFileGenerator;
        private IBulkCoreDataSetAverageCalculator _bulkCoreDataSetAverageCalculator;

        public Program(ILogger logger, IExcelFileDeleter excelFileDeleter,
            IExcelFileGenerator excelFileGenerator, IPracticeProfilesExcelFileGenerator practiceProfilesExcelFileGenerator,
            IBulkCoreDataSetAverageCalculator bulkCoreDataSetAverageCalculator)
        {
            _logger = logger;
            _excelFileDeleter = excelFileDeleter;
            _excelFileGenerator = excelFileGenerator;
            _practiceProfilesExcelFileGenerator = practiceProfilesExcelFileGenerator;
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
                    GenerateExcelFiles();
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
            Console.WriteLine("1- Generate Excel files");
            Console.WriteLine("2- Calculate CoreDataSet Averages");
            Console.WriteLine("3- Exit");
        }

        private void GenerateExcelFiles()
        {
            try
            {
                _excelFileDeleter.DeleteAllExistingFiles();
                _excelFileGenerator.Generate();
                _practiceProfilesExcelFileGenerator.Generate();
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
