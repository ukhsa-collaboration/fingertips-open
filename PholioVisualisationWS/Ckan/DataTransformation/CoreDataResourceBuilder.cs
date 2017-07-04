using System.Collections.Generic;
using Ckan.Model;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public class CoreDataResourceBuilder
    {
        private LookUpManager lookUpManager;

        public CoreDataResourceBuilder(LookUpManager lookUpManager)
        {
            this.lookUpManager = lookUpManager;
        }

        public CkanResource GetUnsavedResource(string packageId, IndicatorMetadata indicatorMetadata,
            IList<CkanCoreDataSet> dataList)
        {
            var indicatorName = indicatorMetadata.Name;

            // Add metadata resource
            var resource = new CkanResource();
            resource.PackageId = packageId;
            resource.Description = "Data for \"" + indicatorName + "\"";
            resource.Format = "CSV";
            resource.Name = "Data";

            // Add file to resource
            byte[] fileContents = GetCoreDataFileAsBytes(dataList);
            var fileNamer = new SingleEntityFileNamer(indicatorName);
            resource.File = new CkanResourceFile
            {
                FileName = fileNamer.DataFileName,
                FileContents = fileContents
            };

            return resource;
        }

        private byte[] GetCoreDataFileAsBytes(IList<CkanCoreDataSet> dataList)
        {
            // Create CSV writer
            var csvWriter = new CsvWriter();
            csvWriter.AddHeader("Area Code", "Area Name", "Area Type", "Sex", "Age",
                "Category Type", "Category",
                "Time period", "Value", "Lower CI limit", "Upper CI limit",
                "Count", "Denominator",
                "Value note");

            // Add descriptive metadata properties
            foreach (CkanCoreDataSet data in dataList)
            {
                var coreDataSet = data.Data;

                var formatter = new CoreDataSetExportFormatter(lookUpManager, coreDataSet);

                var areaCode = coreDataSet.AreaCode;
                csvWriter.AddLine(
                    areaCode,
                    lookUpManager.GetAreaName(areaCode),
                    lookUpManager.GetAreaTypeName(areaCode),
                    lookUpManager.GetSexName(coreDataSet.SexId),
                    lookUpManager.GetAgeName(coreDataSet.AgeId),
                    formatter.CategoryType,
                    formatter.Category,
                    data.TimePeriodString,
                    formatter.Value,
                    formatter.LowerCI,
                    formatter.UpperCI,
                    formatter.Count,
                    formatter.Denominator,
                    formatter.ValueNote);
            }

            byte[] bytes = csvWriter.WriteAsBytes();
            return bytes;
        }
    }
}