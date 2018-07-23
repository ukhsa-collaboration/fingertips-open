using System;
using System.Linq;
using System.Web.Http;
using FingertipsDataExtractionTool.AverageCalculator;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using Unity;
using Unity.Injection;

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
                var container = new UnityContainer();

                GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(LoadUnityContainer(container));
                
                var program = (IProgram)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IProgram));
                program.Go(args);
            }
            catch (Exception ex)
            {
                LogExceptionToConsole(ex);
                ExceptionLog.LogException(ex, null);
            }
        }

        private static UnityContainer LoadUnityContainer(UnityContainer container)
        {
            container.RegisterType<ILogger>(new InjectionFactory(x => LogManager.GetLogger("ExcelFileGenerator")));
            container.RegisterType<IContentItemRepository>(new InjectionFactory(x => ReaderFactory.GetContentItemRepository()));
            container.RegisterType<IProfileReader>(new InjectionFactory(x => ReaderFactory.GetProfileReader()));
            container.RegisterType<IGroupDataReader>(new InjectionFactory(x => ReaderFactory.GetGroupDataReader()));
            container.RegisterType<IAreasReader>(new InjectionFactory(x => ReaderFactory.GetAreasReader()));

            container.RegisterType<IFileDeleter>(new InjectionFactory(x =>
                new FileDeleter(LogManager.GetLogger("ExcelFileGenerator"))));

            container.RegisterType<IPracticePopulationFileGenerator>(new InjectionFactory(x =>
                new PracticePopulationFileGenerator(LogManager.GetLogger("ExcelFileGenertor"),
                    new ExcelFileWriter())));

            container.RegisterType<IAreaTypeListProvider>(new InjectionFactory(x =>
                new AreaTypeListProvider(new GroupIdProvider(ReaderFactory.GetProfileReader()),
                    ReaderFactory.GetAreasReader(),
                    ReaderFactory.GetGroupDataReader())));

            container.RegisterType<IDataFileGenerator>(new InjectionFactory(x =>
                new DataFileGenerator(LogManager.GetLogger("ExcelFileGenerator"),
                    new AreaTypeListProvider(new GroupIdProvider(ReaderFactory.GetProfileReader()),
                        ReaderFactory.GetAreasReader(),
                        ReaderFactory.GetGroupDataReader()), ReaderFactory.GetAreasReader(),
                    ReaderFactory.GetProfileReader())));

            container.RegisterType<IBulkCoreDataSetAverageCalculator>(new InjectionFactory(x =>
                new BulkCoreDataSetAverageCalculator(ReaderFactory.GetAreasReader(),
                    new ParentAreaProvider(ReaderFactory.GetAreasReader(),
                        new AreaTypeListProvider(new GroupIdProvider(ReaderFactory.GetProfileReader()),
                            ReaderFactory.GetAreasReader(), ReaderFactory.GetGroupDataReader())),
                    ReaderFactory.GetGroupDataReader(), new CoreDataSetRepository(),
                    LogManager.GetLogger("ExcelFileGenerator"),
                    new IndicatorMetadataRepository())));

            container.RegisterType<IProgram>(new InjectionFactory(x =>
                new Program(LogManager.GetLogger("ExcelFileGenerator"),
                    new FileDeleter(LogManager.GetLogger("ExcelFileGenerator")),
                    new DataFileGenerator(LogManager.GetLogger("ExcelFileGenerator"),
                        new AreaTypeListProvider(new GroupIdProvider(ReaderFactory.GetProfileReader()),
                            ReaderFactory.GetAreasReader(),
                            ReaderFactory.GetGroupDataReader()), ReaderFactory.GetAreasReader(),
                        ReaderFactory.GetProfileReader()),
                    new PracticePopulationFileGenerator(LogManager.GetLogger("ExcelFileGenertor"),
                        new ExcelFileWriter()),
                    new BulkCoreDataSetAverageCalculator(ReaderFactory.GetAreasReader(),
                        new ParentAreaProvider(ReaderFactory.GetAreasReader(),
                            new AreaTypeListProvider(new GroupIdProvider(ReaderFactory.GetProfileReader()),
                                ReaderFactory.GetAreasReader(),
                                ReaderFactory.GetGroupDataReader())),
                        ReaderFactory.GetGroupDataReader(), new CoreDataSetRepository(),
                        LogManager.GetLogger("ExcelFileGenerator"),
                        new IndicatorMetadataRepository()))));

            return container;
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

            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            // Log to this file only way to see error if the application does not start
            // in task runner
            System.IO.File.AppendAllText(@"C:\fingertips-logs\fdet-error.txt",
                ex.Message + ex.StackTrace);
        }
    }
}
