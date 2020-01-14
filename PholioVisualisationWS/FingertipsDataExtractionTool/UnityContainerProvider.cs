using FingertipsDataExtractionTool.AverageCalculator;
using NLog;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using Unity;

namespace FingertipsDataExtractionTool
{
    public class UnityContainerProvider
    {
        public const string ExcelFileGenerator = "ExcelFileGenerator";

        public static UnityContainer GetUnityContainer()
        {
            var container = new UnityContainer();

            // Types with bespoke creation
            container.RegisterInstance<ILogger>(LogManager.GetLogger(ExcelFileGenerator));
            container.RegisterInstance(ReaderFactory.GetSessionFactory());
            container.RegisterInstance(ReaderFactory.GetContentItemRepository());
            container.RegisterInstance(ReaderFactory.GetProfileReader());
            container.RegisterInstance(ReaderFactory.GetGroupDataReader());
            container.RegisterInstance(ReaderFactory.GetAreasReader());

            // Types created with constructor injection
            container.RegisterType<IFileDeleter, FileDeleter>();
            container.RegisterType<IExcelFileWriter, ExcelFileWriter>();
            container.RegisterType<IPracticePopulationFileGenerator, PracticePopulationFileGenerator>();
            container.RegisterType<IGroupIdProvider, GroupIdProvider>();
            container.RegisterType<IAreaTypeListProvider, AreaTypeListProvider>();
            container.RegisterType<IParentAreaProvider, ParentAreaProvider>();
            container.RegisterType<IDataFileGenerator, DataFileGenerator>();
            container.RegisterType<ICoreDataSetRepository, CoreDataSetRepository>();
            container.RegisterType<IIndicatorMetadataRepository, IndicatorMetadataRepository>();
            container.RegisterType<IBulkCoreDataSetAverageCalculator, BulkCoreDataSetAverageCalculator>();
            container.RegisterType<IProgram, Program>();

            return container;
        }
    }
}