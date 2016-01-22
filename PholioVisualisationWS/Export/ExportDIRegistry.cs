using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PholioVisualisation.DataAccess;
using StructureMap.Configuration.DSL;


namespace PholioVisualisation.Export
{
    public class ExportDIRegistry : Registry
    {
        public ExportDIRegistry()
        {
            // Override injections and any custom parameters constructors if required

            For<IExcelFileWriter>()
                .Use<ExcelFileWriter>()
                .Setter(x => x.UseFileCache).Is(ApplicationConfiguration.UseFileCache);
        }
    }
}
