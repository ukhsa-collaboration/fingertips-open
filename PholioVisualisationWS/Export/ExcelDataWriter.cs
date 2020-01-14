using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    public abstract class ExcelDataWriter
    {
        protected IWorkbook workbook;
        private bool haveOrganisationDetailsBeenAdded;

        protected ExcelDataWriter()
        {
            workbook = Factory.GetWorkbook();
        }

        public IWorkbook Workbook
        {
            get { return workbook; }
        }

        protected virtual string GetDataUrl()
        {
            return null;
        }

        protected virtual IWorksheet IndicatorMetadataWorksheet
        {
            get { throw new NotImplementedException("You need to override IndicatorMetadataWorksheet."); }
        }

        protected virtual int IndicatorMetadataRow
        {
            get { throw new NotImplementedException("You need to override IndicatorMetadataRow."); }
        }

        public void AddOrganisationDetails(int profileId)
        {
            if (haveOrganisationDetailsBeenAdded == false)
            {
                haveOrganisationDetailsBeenAdded = true;

                new CoverSheet().InitCoverSheet(workbook.Worksheets[0], profileId, GetDataUrl());
            }
        }

        protected static void SetColumnAsText(IRange cell, string title)
        {
            cell.Value = title;
            cell.EntireColumn.HorizontalAlignment = HAlign.Left;
            cell.EntireColumn.NumberFormat = "@";
        }

        protected static void SetColumnWidths(IRange row, int[] columnWidths)
        {
            for (int i = 0; i < columnWidths.Length; i++)
            {
                row.Columns[0, i].ColumnWidth = columnWidths[i];
            }
        }

        public virtual void FinaliseBeforeWrite()
        {
        }

        public void AddIndicatorMetadata(IList<IndicatorMetadata> metadata)
        {
            IWorksheet ws = IndicatorMetadataWorksheet;
            ws.Name = "Indicator Metadata";

            // Property names to export
            IOrderedEnumerable<IndicatorMetadataTextProperty> propertiesToDisplay = GetPropertiesToDisplay(metadata);

            // Headers
            AddIndicatorMetadataHeader(ws, propertiesToDisplay);

            // Data
            HtmlCleaner htmlCleaner = new HtmlCleaner();
            IRange cells = ws.Cells;
            int metadataRow = IndicatorMetadataRow;
            foreach (IndicatorMetadata indicatorMetadata in metadata)
            {
                IDictionary<string, string> textMetadata = indicatorMetadata.Descriptive;
                int column = 0;
                cells[metadataRow, column++].Value = textMetadata[IndicatorMetadataTextColumnNames.Name];

                // Definition may be null
                string val;
                if (textMetadata.TryGetValue(IndicatorMetadataTextColumnNames.Definition, out val) == false)
                {
                    val = string.Empty;
                }
                cells[metadataRow, column++].Value = htmlCleaner.RemoveHtml(val);

                cells[metadataRow, column++].Value = indicatorMetadata.ValueType.Name;
                cells[metadataRow, column++].Value = indicatorMetadata.Unit.Label;

                foreach (var property in propertiesToDisplay)
                {
                    if (textMetadata.ContainsKey(property.ColumnName))
                    {
                        string text = textMetadata[property.ColumnName];

                        if (property.IsHtml)
                        {
                            // Remove HTML
                            text = htmlCleaner.RemoveHtml(text);
                        }

                        cells[metadataRow, column].Value = text;
                    }
                    column++;
                }

                IRange range = cells.Cells[metadataRow, 0, metadataRow, column];
                range.WrapText = true;
                range.VerticalAlignment = VAlign.Top;
                metadataRow++;
            }
        }

        private IOrderedEnumerable<IndicatorMetadataTextProperty> GetPropertiesToDisplay(IList<IndicatorMetadata> metadata)
        {
            List<string> names = new List<string>();
            var nameLists = (from m in metadata select m.Descriptive.Keys);
            foreach (var nameList in nameLists)
            {
                names.AddRange(nameList);
            }
            names = names.Distinct().ToList();

            names.Remove(IndicatorMetadataTextColumnNames.Name);
            names.Remove(IndicatorMetadataTextColumnNames.Definition);

            IList<IndicatorMetadataTextProperty> properties = IndicatorMetadataProvider.Instance.IndicatorMetadataTextProperties;
            return properties.Where(p => names.Contains(p.ColumnName)).Where(p=>p.IsSystemContent==false).OrderBy(p => p.DisplayOrder);
        }

        private static void AddIndicatorMetadataHeader(IWorksheet ws, IEnumerable<IndicatorMetadataTextProperty> properties)
        {
            IRange row = ws.Cells[0, 0, 0, properties.Count() + 4];
            row.Font.Bold = true;
            int column = 0;
            row[0, column++].Value = "Indicator";
            row[0, column++].Value = IndicatorMetadataTextColumnNames.Definition;
            row[0, column++].Value = "Value type";
            row[0, column++].Value = "Unit";

            foreach (var property in properties)
            {
                row[0, column++].Value = property.DisplayName;
            }

            List<int> sizes = new List<int> { 40, 65, 12, 12 };

            for (int propertyIndex = 0; propertyIndex < properties.Count(); propertyIndex++)
            {
                sizes.Add(35);
            }

            SetColumnWidths(row, sizes.ToArray());
        }
    }
}
