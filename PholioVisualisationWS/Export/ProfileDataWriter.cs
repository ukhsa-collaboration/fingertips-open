using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    public class ProfileDataWriter : ExcelDataWriter
    {
        private readonly Profile profile;

        private readonly Dictionary<string, WorksheetInfo> wsDictionary = new Dictionary<string, WorksheetInfo>();

        public ProfileDataWriter(Profile profile)
        {
            this.profile = profile;

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
            if (profile.IsDefinedProfile)
            {
                profileUrl = profileUrl.TrimEnd('/') + "/" + profile.UrlKey;
            }
            return profileUrl;
        }

        public void AddSheet(string sheetLabel)
        {
            var ws = new WorksheetInfo {Worksheet = workbook.Worksheets.Add()};
            ws.Worksheet.Name = sheetLabel;
            wsDictionary.Add(sheetLabel, ws);
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

            SetColumnAsText(row[rowIndex, column], "Note");

            foreach (string range in new[] {"$G:$G", "$H:$I", "$I:$I", "$J:$J", "$K:$K"})
            {
                ws.Worksheet.Cells[range].NumberFormat = "0.00";
            }

            row.Font.Bold = true;

            SetColumnWidths(row, new[] {45, 13, 10, 20, 10, 20, 13, 13, 13, 13, 13, 15, 15, 35});
        }

        public void AddCategorisedData(string sheetLabel, string timePeriod, IList<CoreDataSet> coreData,
            Dictionary<int, IArea> categoryIdToAreaMap, string sex, IDictionary<string, string> metadata,
            string age, IList<ValueNote> valueNotes)
        {
            WorksheetInfo ws = wsDictionary[sheetLabel];
            IRange cells = ws.Worksheet.Cells;
            foreach (CoreDataSet d in coreData)
            {
                IArea area = categoryIdToAreaMap[d.CategoryId];

                ValueNote valueNote = valueNotes.FirstOrDefault(x => x.Id == d.ValueNoteId);

                AddDataRow(ws, area, d, cells, metadata, timePeriod, sex, age, valueNote, null, null);
            }
        }

        public void AddData(string sheetLabel, string timePeriod, IList<CoreDataSet> coreData, 
            Dictionary<string, IArea> areaCodeToAreaMap, string sex, IDictionary<string, string> metadata, string age, IList<ValueNote> valueNotes, Dictionary<string, Area> areaCodeToParentMap)
        {
            WorksheetInfo ws = wsDictionary[sheetLabel];
            IRange cells = ws.Worksheet.Cells;
            foreach (CoreDataSet d in coreData)
            {
                IArea area = areaCodeToAreaMap[d.AreaCode];
                Area parentArea;

                string parentAreaName="",
                    parentAreaCode="";

                if (areaCodeToParentMap.TryGetValue(d.AreaCode, out parentArea))
                {
                    parentAreaCode = areaCodeToParentMap[d.AreaCode].Code;
                    parentAreaName = areaCodeToParentMap[d.AreaCode].Name;
                }

                ValueNote valueNote = valueNotes.FirstOrDefault(x => x.Id == d.ValueNoteId);

                AddDataRow(ws, area, d, cells, metadata, timePeriod, sex, age, valueNote, parentAreaCode, parentAreaName);
            }
        }

        public void AddData(string worksheetLabel, string timePeriod, CoreDataSet coreData, IArea area, string sex,
            IDictionary<string, string> metadata, string age, IList<ValueNote> valueNotes)
        {
            WorksheetInfo ws = wsDictionary[worksheetLabel];
            IRange cells = ws.Worksheet.Cells;

            ValueNote valueNote = valueNotes.FirstOrDefault(x => x.Id == coreData.ValueNoteId);
            AddDataRow(ws, area, coreData, cells, metadata, timePeriod, sex, age, valueNote, null, null);
        }

        private static void AddDataRow(WorksheetInfo ws, IArea area, CoreDataSet coreData, IRange cells, IDictionary<string, string> metadata, string timePeriod, string sex, string age, ValueNote valueNote, string parentAreaCode, string parentAreaName)
        {

            string areaCode;
            string areaName;

            bool isCategory = false;
            if (CategoryArea.IsCategoryAreaCode(area.Code))
            {
                var areaCategory = (CategoryArea)area;
                areaCode = areaCategory.CategoryId.ToString();
                areaName = areaCategory.Name;
                isCategory = true;
            }
            else
            {
                areaCode = area.Code;
                areaName = area.Name;
            }

            try
            {
                int rowIndex = ws.NextRow;
                int column = 0;
                
                cells[rowIndex, column++].Value = metadata[IndicatorMetadataTextColumnNames.Name];
                cells[rowIndex, column++].Value = timePeriod;

                cells[rowIndex, column++].Value = parentAreaCode ?? string.Empty;
                cells[rowIndex, column++].Value = parentAreaName ?? string.Empty;

                if (isCategory)
                {
                    AddValue(cells[rowIndex, column++], Convert.ToDouble(areaCode) );
                }
                else
                {
                    cells[rowIndex, column++].Value = areaCode;    
                }
                
                cells[rowIndex, column++].Value = areaName;

                AddValue(cells[rowIndex, column++], coreData.Value);
                AddValue(cells[rowIndex, column++], coreData.LowerCI);
                AddValue(cells[rowIndex, column++], coreData.UpperCI);

                if (coreData.IsCountValid)
                {
                    AddValue(cells[rowIndex, column], coreData.Count.Value);
                }
                column++;

                AddValue(cells[rowIndex, column++], coreData.Denominator);

                cells[rowIndex, column++].Value = sex;
                cells[rowIndex, column++].Value = age;

                if (coreData.ValueNoteId > 0)
                {
                    cells[rowIndex, column].Value = valueNote.Text;
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
            foreach (var worksheetInfo in wsDictionary)
            {
                if (worksheetInfo.Value.IsWorksheetEmpty)
                {
                    worksheetInfo.Value.Worksheet.Delete();
                }
            }
        }
    }
}