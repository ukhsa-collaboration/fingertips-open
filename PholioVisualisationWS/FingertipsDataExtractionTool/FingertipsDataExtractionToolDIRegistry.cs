using NHibernate;
using StructureMap.Configuration.DSL;
using NLog;
using PholioVisualisation.DataAccess.Repositories;

namespace FingertipsDataExtractionTool
{
    public class FingertipsDataExtractionToolDIRegistry : Registry
    {
        public FingertipsDataExtractionToolDIRegistry()
        {
            For<IProgram>().Use<Program>();
          
            For<ILogger>()
                .Use(x => LogManager.GetLogger("ExcelFileGenerator"));

            For<ISessionFactory>().Use(x => NHibernateSessionFactory.GetSession());
        }  
    }
}