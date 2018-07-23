using System;
using System.IO;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;
using SpreadsheetGear.Drawing;

namespace PholioVisualisation.Export
{
    public class CoverSheet
    {
        private const double PixelsToPoints = 72.0 / 96.0;

        public void InitCoverSheet(IWorksheet ws, int profileId, string dataPageUrl)
        {
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
                string dataUrl = dataPageUrl;
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
            string path = Path.Combine(ApplicationConfiguration.Instance.ImagesDirectory, fileName);
            System.Drawing.Image i = System.Drawing.Image.FromFile(path);
            ws.Shapes.AddPicture(path, left, top, i.Width * PixelsToPoints, i.Height * PixelsToPoints);
        }
    }
}