using StructureMap.Configuration.DSL;
using NLog;

namespace FingertipsDataExtractionTool
{
    public class FingertipsDataExtractionToolDIRegistry : Registry
    {
        public FingertipsDataExtractionToolDIRegistry()
        {
            For<IProgram>().Use<Program>();
          
            For<ILogger>()
                .Use<Logger>(x => LogManager.GetLogger("ExcelFileGenerator"));
        }  
    }
}