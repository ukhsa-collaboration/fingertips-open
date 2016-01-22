using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;
using SpreadsheetGear.Drawing;

namespace PholioVisualisation.Export
{
    public abstract class ExcelDataWriter
    {
        private const double PixelsToPoints = 72.0 / 96.0;

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

                IWorksheet ws = workbook.Worksheets[0];
                ws.Cells.Borders.Color = System.Drawing.Color.White;
                IFont font = ws.Cells.Font;
                font.Name = "verdana";
                font.Color = Color.FromArgb(45, 45, 45);
                font.Size = 10;

                int row = 10;
                const int col1 = 1;
                int col2 = col1 + 2;

                IRange cell = ws.Cells[row, col1];
                cell.Value = "Website:";
                IRange cell2 = ws.Cells[row, col2];

                if (profileId == ProfileIds.Phof)
                {
                    ws.Name = "PHOF";

                    AddWebAddress(cell2, "www.phoutcomes.info");
                    row++;

                    AddAddress(ws, row, col1, col2);
                    row++;

                    ws.Cells[row, col1].Value = "Email:";
                    ws.Cells[row, col2].Value = "PHOF.enquiries@phe.gov.uk";
                    row += 2;

                    ws.Cells[row, col1].Value = "Data:";
                    ws.Cells[row, col2].Value = "Public Health Outcomes Framework data";
                    row++;
                }
                else if (profileId == ProfileIds.Tobacco)
                {
                    ws.Name = "Tobacco";

                    AddWebAddress(cell2, "www.tobaccoprofiles.info");
                    row++;

                    AddAddress(ws, row, col1, col2);
                    row++;

                    ws.Cells[row, col1].Value = "Email:";
                    ws.Cells[row, col2].Value = "londonkit@phe.gov.uk";
                    row += 2;

                    ws.Cells[row, col1].Value = "Data:";
                    ws.Cells[row, col2].Value = "Local Tobacco Control data";
                    row++;
                }

                else
                {
                    ws.Name = "PHE";

                    AddWebAddress(cell2, "fingertips.phe.org.uk");
                    row++;

                    AddAddress(ws, row, col1, col2);
                    row++;

                    ws.Cells[row, col1].Value = "Email:";
                    ws.Cells[row, col2].Value = "profilefeedback@phe.gov.uk";
                    row += 2;

                    // Data link
                    string dataUrl = GetDataUrl();
                    if (string.IsNullOrEmpty(dataUrl) == false)
                    {
                        ws.Cells[row, col1].Value = "Data Link:";
                        IRange linkCell = ws.Cells[row, col2];
                        linkCell.Hyperlinks.Add(linkCell, dataUrl, "", "", dataUrl);
                    }

                    row++;
                }

                AddPicture(ws, 50/*left*/, 40/*top*/, "PHE_logo.png");

                ws.Cells[row, col1].Value = "Date generated:";
                ws.Cells[row, col2].Value = DateTime.Now.ToString("dd MMM yyyy");

                IRange column = cell.EntireColumn;
                column.ColumnWidth = 17;
                column.HorizontalAlignment = HAlign.Right;
                column.Font.Size = 9;
                column.Font.Italic = true;

                ws.Cells[row, col1 + 1].EntireColumn.ColumnWidth = 3;

                cell2.EntireColumn.AutoFit();
                cell2.EntireColumn.HorizontalAlignment = HAlign.Left;
            }
        }

        private static void AddAddress(IWorksheet ws, int row, int col1, int col2)
        {
            ws.Cells[row, col1].Value = "Address:";
            ws.Cells[row, col2].Value = "Public Health England, Wellington House, 133-155 Waterloo Road, London, SE1 8UG";
        }

        private static void AddWebAddress(IRange cell2, string address)
        {
            cell2.Hyperlinks.Add(cell2, @"http://" + address, "", "", address);
        }

        private static void AddPicture(IWorksheet ws, int left, int top, string fileName)
        {
            string path = Path.Combine(ApplicationConfiguration.ImagesDirectory, fileName);
            System.Drawing.Image i = System.Drawing.Image.FromFile(path);
            ws.Shapes.AddPicture(path, left, top, i.Width * PixelsToPoints, i.Height * PixelsToPoints);
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
                IDictionary<string, string> m = indicatorMetadata.Descriptive;
                int column = 0;
                cells[metadataRow, column++].Value = m[IndicatorMetadataTextColumnNames.Name];

                // Definition may be null
                string val;
                if (m.TryGetValue(IndicatorMetadataTextColumnNames.Definition, out val) == false)
                {
                    val = string.Empty;
                }
                cells[metadataRow, column++].Value = htmlCleaner.RemoveHtml(val);

                cells[metadataRow, column++].Value = indicatorMetadata.ValueType.Label;
                cells[metadataRow, column++].Value = indicatorMetadata.Unit.Label;

                foreach (var property in propertiesToDisplay)
                {
                    if (m.ContainsKey(property.ColumnName))
                    {
                        string text = m[property.ColumnName];

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
            names.Remove(IndicatorMetadataTextColumnNames.NameLong);
            names.Remove(IndicatorMetadataTextColumnNames.Definition);

            IList<IndicatorMetadataTextProperty> properties = IndicatorMetadataRepository.Instance.IndicatorMetadataTextProperties;
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
