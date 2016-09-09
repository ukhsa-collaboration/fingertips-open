using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    public class ProfileDataWriter : ExcelDataWriter
    {
        private readonly Profile _profile;
        private readonly Dictionary<string, WorksheetInfo> _wsDictionary = new Dictionary<string, WorksheetInfo>();

        public ProfileDataWriter(Profile profile)
        {
            _profile = profile;

            // Indicator metadata
            workbook.Worksheets.Add();
        }

        protected ProfileDataWriter()
        {
        }

        protected override IWorksheet IndicatorMetadataWorksheet
        {
            get { return workbook.Worksheets[1]; }
        }

        protected override int IndicatorMetadataRow
        {
            get { return 1; }
        }

        protected override string GetDataUrl()
        {
            string profileUrl = ApplicationConfiguration.UrlUI;
            if (_profile.IsDefinedProfile)
            {
                profileUrl = profileUrl.TrimEnd('/') + "/" + _profile.UrlKey;
            }
            return profileUrl;
        }

        public WorksheetInfo GetWorksheetInfo(string sheetName)
        {
            if (string.IsNullOrEmpty(sheetName) || _wsDictionary.ContainsKey(sheetName) == false)
            {
                return null;
            }
            return _wsDictionary[sheetName];
        }

        public void AddSheet(string sheetLabel)
        {
            var ws = new WorksheetInfo { Worksheet = workbook.Worksheets.Add() };
            ws.Worksheet.Name = sheetLabel;
            _wsDictionary.Add(sheetLabel, ws);
            AddDataHeader(ws);
        }

        private static void AddDataHeader(WorksheetInfo ws)
        {
            int rowIndex = ws.NextRow;
            IRange row = ws.Worksheet.Cells[rowIndex, 0, rowIndex, 13];
            int column = 0;
            row[rowIndex, column++].Value = "Indicator";
            SetColumnAsText(row[rowIndex, column++], "Time Period");
            SetColumnAsText(row[rowIndex, column++], "Parent Code");
            SetColumnAsText(row[rowIndex, column++], "Parent Name");
            SetColumnAsText(row[rowIndex, column++], "Area Code");
            row[rowIndex, column++].Value = "Area Name";
            row[rowIndex, column++].Value = "Value";
            row[rowIndex, column++].Value = "Lower CI";
            row[rowIndex, column++].Value = "Upper CI";

            row[rowIndex, column++].Value = "Count";
            row[rowIndex, column++].Value = "Denominator";
            SetColumnAsText(row[rowIndex, column++], "Sex");
            SetColumnAsText(row[rowIndex, column++], "Age");

            SetColumnAsText(row[rowIndex, column++], "Note");
            SetColumnAsText(row[rowIndex, column], "Recent Trend");

            foreach (string range in new[] { "$G:$G", "$H:$I", "$I:$I", "$J:$J", "$K:$K" })
            {
                ws.Worksheet.Cells[range].NumberFormat = "0.00";
            }

            row.Font.Bold = true;

            SetColumnWidths(row, new[] { 45, 13, 10, 20, 10, 20, 13, 13, 13, 13, 13, 15, 15, 35, 30 });
        }

        public void AddCategorisedData(WorksheetInfo ws, RowLabels rowLabels, IList<CoreDataSet> dataList,
            Dictionary<int, IArea> categoryIdToAreaMap)
        {
            IRange cells = ws.Worksheet.Cells;
            foreach (CoreDataSet data in dataList)
            {
                IArea area = categoryIdToAreaMap[data.CategoryId];
                AddDataRow(ws, rowLabels, area, data, cells, null, null);
            }
        }

        public void AddData(WorksheetInfo ws, RowLabels rowLabels, IList<CoreDataSet> dataList,
            Dictionary<string, IArea> areaCodeToAreaMap, Dictionary<string, Area> areaCodeToParentMap)
        {
            IRange cells = ws.Worksheet.Cells;
            foreach (CoreDataSet data in dataList)
            {
                IArea area = areaCodeToAreaMap[data.AreaCode];
                Area parentArea;

                string parentAreaName = "",
                    parentAreaCode = "";

                if (areaCodeToParentMap.TryGetValue(data.AreaCode, out parentArea))
                {
                    parentAreaCode = areaCodeToParentMap[data.AreaCode].Code;
                    parentAreaName = areaCodeToParentMap[data.AreaCode].Name;
                }

                AddDataRow(ws, rowLabels, area, data, cells, parentAreaCode, parentAreaName);
            }
        }

        public void AddData(WorksheetInfo ws, RowLabels rowLabels, CoreDataSet coreData, IArea area)
        {
            if (coreData != null)
            {
                IRange cells = ws.Worksheet.Cells;
                AddDataRow(ws, rowLabels, area, coreData, cells, null, null);
            }
        }

        public void AddTrendMarker(TrendMarkerLabel trendMarkerLabel, int rowOffset, WorksheetInfo ws)
        {
            int currentRowIndex = ws.CurrentRow;
            int column = 0;
            IRange cells = ws.Worksheet.Cells;
            int rowIndex = currentRowIndex - rowOffset;
            if (rowIndex > 1)
            {
                cells[rowIndex, ColumnIndexes.RecentTrend].Value = trendMarkerLabel.Text;
            }
        }

        private static void AddDataRow(WorksheetInfo ws, RowLabels rowLabels, IArea area, CoreDataSet coreData,
            IRange cells, string parentAreaCode, string parentAreaName)
        {
            try
            {
                int rowIndex = ws.NextRow;
                int column = 0;

                // Indicator / time period
                cells[rowIndex, column++].Value = rowLabels.IndicatorName;
                cells[rowIndex, column++].Value = rowLabels.TimePeriod;

                // Parent area
                cells[rowIndex, column++].Value = parentAreaCode ?? string.Empty;
                cells[rowIndex, column++].Value = parentAreaName ?? string.Empty;

                // Area code
                var category = area as CategoryArea;
                if (category != null)
                {
                    AddValue(cells[rowIndex, column++], Convert.ToDouble(category.CategoryId));
                }
                else
                {
                    cells[rowIndex, column++].Value = area.Code;
                }

                // Area name
                cells[rowIndex, column++].Value = area.Name;

                // Value / CIs
                AddValue(cells[rowIndex, column++], coreData.Value);
                AddValue(cells[rowIndex, column++], coreData.LowerCI);
                AddValue(cells[rowIndex, column++], coreData.UpperCI);

                // Count
                if (coreData.IsCountValid)
                {
                    AddValue(cells[rowIndex, column], coreData.Count.Value);
                }
                column++;

                // Denominator
                AddValue(cells[rowIndex, column++], coreData.Denominator);

                // Sex / age
                cells[rowIndex, column++].Value = rowLabels.Sex;
                cells[rowIndex, column++].Value = rowLabels.Age;

                // Value note
                if (coreData.ValueNoteId > 0)
                {
                    cells[rowIndex, column].Value = rowLabels.ValueNoteLookUp[coreData.ValueNoteId];
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.LogException(ex, "");
            }
        }

        private static void AddValue(IRange cells, double val)
        {
            if (val != ValueData.NullValue)
            {
                cells.Value = val;
            }
        }

        public override void FinaliseBeforeWrite()
        {
            DeleteEmptyWorksheets();
        }

        private void DeleteEmptyWorksheets()
        {
            foreach (var worksheetInfo in _wsDictionary)
            {
                if (worksheetInfo.Value.IsWorksheetEmpty)
                {
                    worksheetInfo.Value.Worksheet.Delete();
                }
            }
        }
    }
}