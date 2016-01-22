using System.Collections.Generic;
using Ckan.Model;
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
            IDictionary<string, string> descriptive = indicatorMetadata.Descriptive;
            var indicatorName = descriptive[IndicatorMetadataTextColumnNames.Name];

            // Add metadata resource
            var resource = new CkanResource();
            resource.PackageId = packageId;
            resource.Description = "Data for \"" + indicatorName + "\"";
            resource.Format = "CSV";
            resource.Name = "Data";

            // Add file to resource
            byte[] fileContents = GetCoreDataFileAsBytes(dataList);
            var fileNamer = new CkanFileNamer(indicatorName);
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

                string categoryTypeId = coreDataSet.CategoryTypeId != -1
                    ? lookUpManager.GetCategoryTypeName(coreDataSet.CategoryTypeId)
                    : "";

                string categoryId = coreDataSet.CategoryId != -1
                    ? lookUpManager.GetCategoryName(coreDataSet.CategoryTypeId, coreDataSet.CategoryId)
                    : "";

                var val = coreDataSet.IsValueValid
                    ? coreDataSet.Value.ToString()
                    : "";

                string lowerCI;
                string upperCI;
                if (coreDataSet.AreCIsValid)
                {
                    lowerCI = coreDataSet.LowerCI.ToString();
                    upperCI = coreDataSet.UpperCI.ToString();
                }
                else
                {
                    lowerCI = string.Empty;
                    upperCI = string.Empty;
                }

                var count = coreDataSet.IsCountValid
                    ? coreDataSet.Count.ToString()
                    : "";

                var denominator = coreDataSet.IsDenominatorValid
                    ? coreDataSet.Denominator.ToString()
                    : "";

                var valueNote = coreDataSet.ValueNoteId > 0
                    ? lookUpManager.GetValueNoteText(coreDataSet.ValueNoteId)
                    : "";

                var areaCode = coreDataSet.AreaCode;
                csvWriter.AddLine(
                    areaCode,
                    lookUpManager.GetAreaName(areaCode),
                    lookUpManager.GetAreaTypeName(areaCode),
                    lookUpManager.GetSexName(coreDataSet.SexId),
                    lookUpManager.GetAgeName(coreDataSet.AgeId),
                    categoryTypeId,
                    categoryId,
                    data.TimePeriodString,
                    val,
                    lowerCI,
                    upperCI,
                    count,
                    denominator,
                    valueNote);
            }

            byte[] bytes = csvWriter.WriteAsBytes();
            return bytes;
        }
    }
}