/* 
 * Created by: Daniel Flint    
 * Date: 19/07/2011 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErphoBusiness.DataAccess;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    internal class IndicatorIndexWriter : ExcelFileWriter
    {
        private IWorksheet ws;
        private int rowIndex;
        private bool hasDomains;
        private SheetNamer sheetNamer = new SheetNamer();

        public void InitNewProfile(Profile profile, bool hasDomains)
        {
            AddOrganisationDetails(profile.Id);

            this.hasDomains = hasDomains;

            ws = workbook.Worksheets.Add();
            ws.Name = sheetNamer.GetSheetName(profile.Name);

            rowIndex = 0;
            AddHeader();
        }

        private void AddHeader()
        {
            IRange row = ws.Cells[0, 0, 0, 6];
            row.Font.Bold = true;
            int column = 0;
            if (hasDomains)
            {
                row[0, column++].Value = "Domain";
            }

            row[0, column++].Value = "Indicator";
            SetColumnAsText(row[rowIndex, column++], "Latest Date");
            row[0, column++].Value = "Unit";
            row[0, column++].Value = "Value type";
            row[0, column].Value = "Data source";

            rowIndex++;

            List<int> widths = new List<int> { 50, 60, 15, 16, 30, 80 };
            if (hasDomains == false)
            {
                widths.RemoveAt(0);
            }

            SetColumnWidths(row, widths.ToArray());
        }

        public void AddIndicator(IndicatorMetadata metadata, string dataPointTime, string domainName)
        {
            IRange cells = ws.Cells;
            IDictionary<string, string> m = metadata.Descriptive;
            try
            {
                int column = 0;
                if (hasDomains)
                {
                    cells[rowIndex, column++].Value = domainName;
                }
                cells[rowIndex, column++].Value = m[IndicatorMetadataTextColumnNames.Name];
                cells[rowIndex, column++].Value = dataPointTime;
                cells[rowIndex, column++].Value = metadata.Unit.Label;
                cells[rowIndex, column++].Value = metadata.ValueType.Label;
                cells[rowIndex, column].Value = m[IndicatorMetadataTextColumnNames.DataSource].Trim();
                rowIndex++;
            }
            catch (Exception ex)
            {
                new DatabaseLogger().LogException(ex, "");
            }
        }

    }
}
