using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    /// <summary>
    /// Writes an Excel file of Practice Profile data.
    /// </summary>
    internal class PracticeProfileDataWriter : ExcelDataWriter
    {
        private const string HeaderPracticeCode = "Practice Code";
        private const string HeaderCcgName = "CCG Name";
        private const string HeaderCountyUaName = "County UA Name";

        private const int StartingDataRow = 3;
        private const int ColumnWidthAreaCode = 10;
        private int ccgPopulationDataRow = StartingDataRow;
        private const int ColumnsPerIndicator = 5;

        // List of area codes used to ensure order of data is consistent
        private IList<string> practiceCodes;
        private IList<string> parentAreaCodes;

        // The current column for writing data
        private int practiceDataColumn;

        private int parentAreaDataColumn;

        private int areaTypeId;

        public PracticeProfileDataWriter(int areaTypeId)
        {
            this.areaTypeId = areaTypeId;
            PracticeDataWorksheet.Name = "Practice";

            IWorksheet ws = workbook.Worksheets.Add();
            var areasReader = ReaderFactory.GetAreasReader();
            var areaType = areasReader.GetAreaType(areaTypeId);
            ws.Name = new SheetNamer().GetSheetName(areaType.ShortName);

            ws = workbook.Worksheets.Add();
            ws.Name = "Indicator Metadata";

            ws = workbook.Worksheets.Add();
            ws.Name = "Practice Addresses";
        }

        private IWorksheet PracticeDataWorksheet
        {
            get { return workbook.Worksheets[0]; }
        }

        private IWorksheet parentAreaDataWorksheet
        {
            get { return workbook.Worksheets[1]; }
        }

        private IWorksheet ParentAreaDataWorksheet
        {
            get { return workbook.Worksheets[1]; }
        }

        protected override IWorksheet IndicatorMetadataWorksheet
        {
            get { return workbook.Worksheets[2]; }
        }

        private IWorksheet PracticeAddressWorksheet
        {
            get { return workbook.Worksheets[3]; }
        }

        public void AddPracticeAddresses(Dictionary<string, AreaAddress> practiceCodeToAddressMap)
        {
            IWorksheet ws = PracticeAddressWorksheet;

            AddPracticeAddressHeader(ws);

            // Addresses
            IRange cells = ws.Cells;
            int row = StartingDataRow;
            try
            {
                foreach (string code in practiceCodes)
                {
                    int col = 0;
                    if (practiceCodeToAddressMap.ContainsKey(code))
                    {
                        AreaAddress address = practiceCodeToAddressMap[code];

                        cells[row, col++].Value = code;
                        cells[row, col++].Value = address.Name;
                        cells[row, col++].Value = new AddressStringifier(address).AddressWithoutPostcode;
                        cells[row, col].Value = address.Postcode;
                    }
                    row++;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.LogException(ex, string.Empty);
            }
        }

        private static void AddPracticeAddressHeader(IWorksheet ws)
        {
            AddDataHeader(ws, new[] { 
                new ColumnHeader { Title = HeaderPracticeCode, Width = ColumnWidthAreaCode },
                new ColumnHeader { Title = "Practice Name", Width = 40 },
                new ColumnHeader { Title = "Address", Width = 80 },
                new ColumnHeader { Title = "Postcode", Width = 15 }
            });
        }

        public void AddPracticeCodes(IList<string> codes)
        {
            practiceCodes = codes;
            IWorksheet ws = PracticeDataWorksheet;

            AddDataHeader(ws, new[] { 
                new ColumnHeader { Title = HeaderPracticeCode, Width = ColumnWidthAreaCode },
                new ColumnHeader { Title = areaTypeId==AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 ? HeaderCountyUaName : HeaderCcgName, Width = 40 }
            });

            // Data
            IRange cells = ws.Cells;
            int row = StartingDataRow;
            foreach (string code in codes)
            {
                cells[row++, practiceDataColumn].Value = code;
            }

            practiceDataColumn++;
        }

        public void AddAreaNamesCodestoParentAreaSheet(IEnumerable<Area> parentAreas, int areaTypeId)
        {
            IWorksheet ws = ParentAreaDataWorksheet;

            AddDataHeader(ws, new[] { 
                new ColumnHeader { Title = areaTypeId==AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019 ? HeaderCountyUaName : HeaderCcgName, Width = 60 }
            });

            parentAreaCodes = (from a in parentAreas select a.Code).ToList();

            // Data
            IRange cells = ws.Cells;
            int row = StartingDataRow;
            foreach (Area area in parentAreas)
            {
                cells[row, parentAreaDataColumn].Value = area.Name;
                row++;
            }

            parentAreaDataColumn++;
        }

        public void AddParentsToPracticeSheet(Dictionary<string, Area> practiceToParentMap, bool showCode)
        {
            if (practiceToParentMap != null)
            {
                // Data
                IWorksheet ws = PracticeDataWorksheet;
                IRange cells = ws.Cells;
                int row = StartingDataRow;
                foreach (string code in practiceCodes)
                {
                    if (practiceToParentMap.ContainsKey(code))
                    {
                        Area parentArea = practiceToParentMap[code];
                        if (showCode)
                        {
                            cells[row, practiceDataColumn].Value = parentArea.Code;
                        }
                        else
                        {
                            cells[row, practiceDataColumn].Value = parentArea.Name;
                        }
                    }
                    row++;
                }
            }

            practiceDataColumn++;
        }

        protected override int IndicatorMetadataRow
        {
            get { return 1; }
        }

        public void AddPracticeIndicatorTitles(IndicatorMetadata metadata, IList<TimePeriod> periods)
        {
            AddIndicatorTitle(practiceDataColumn, periods, PracticeDataWorksheet, metadata);
        }

        public void AddCcgIndicatorTitles(IndicatorMetadata metadata, IList<TimePeriod> periods)
        {
            AddIndicatorTitle(parentAreaDataColumn, periods, ParentAreaDataWorksheet, metadata);
        }

        public void AddPopulationTitles(IList<string> sexNames, IList<string> timePeriods, IList<string> populationLabels)
        {
            AddPopulationTitlesToSheet(sexNames, timePeriods, populationLabels, PracticeDataWorksheet, practiceDataColumn);
            AddPopulationTitlesToSheet(sexNames, timePeriods, populationLabels, ParentAreaDataWorksheet, parentAreaDataColumn);
        }

        private static void AddPopulationTitlesToSheet(IList<string> sexNames, IList<string> timePeriods,
            IList<string> populationLabels, IWorksheet ws, int startColumn)
        {
            int columnsPerSex = populationLabels.Count;
            int columnPerPeriod = columnsPerSex * sexNames.Count;

            const int sexRow = 1;
            const int timeRow = 0;
            const int ageRow = 2;

            int sexColumn = startColumn;
            int ageColumn = startColumn;
            int timeColumn = startColumn;

            foreach (var timePeriod in timePeriods)
            {
                int endTimeColumn = timeColumn + columnPerPeriod - 1;
                IRange timeRange = ws.Cells[timeRow, timeColumn, timeRow, endTimeColumn];
                timeRange.Merge();
                FormatAsTitle(timeRange);
                timeRange.Value = timePeriod;
                timeColumn += columnPerPeriod;

                foreach (var sexName in sexNames)
                {
                    int endSexColumn = sexColumn + columnsPerSex - 1;
                    IRange sexRange = ws.Cells[sexRow, sexColumn, sexRow, endSexColumn];
                    sexRange.Merge();
                    FormatAsTitle(sexRange);
                    sexRange.Value = sexName;

                    sexColumn += columnsPerSex;

                    foreach (var populationLabel in populationLabels)
                    {
                        IRange ageRange = ws.Cells[ageRow, ageColumn++];
                        FormatAsTitle(ageRange);
                        ageRange.NumberFormat = "@";
                        ageRange.Value = populationLabel;
                    }
                }
            }
        }

        private static void AddIndicatorTitle(int startCol, IList<TimePeriod> periods, IWorksheet ws, IndicatorMetadata metadata)
        {
            int col = startCol;
            int endColumn = col + (periods.Count * ColumnsPerIndicator) - 1;
            IRange range = ws.Cells[0, col, 0, endColumn];
            range.Merge();
            range.Value = metadata.Descriptive[IndicatorMetadataTextColumnNames.Name];

            // Time
            const int dateRowIndex = 1;
            const int labelRowIndex = 2;

            var timePeriodFormatter = new TimePeriodTextFormatter(metadata);

            foreach (TimePeriod timePeriod in periods)
            {
                var endColumnForPeriod = col + ColumnsPerIndicator - 1;
                // Time title
                range = ws.Cells[dateRowIndex, col, dateRowIndex, endColumnForPeriod];
                range.Merge();
                range.Value = timePeriodFormatter.Format(timePeriod);

                // Val, LCI, UCI, Count, Denominator
                ws.Cells[labelRowIndex, col++].Value = "Value";
                ws.Cells[labelRowIndex, col++].Value = "Lower CI";
                ws.Cells[labelRowIndex, col++].Value = "Upper CI";
                ws.Cells[labelRowIndex, col++].Value = "Count";
                ws.Cells[labelRowIndex, col++].Value = "Denom";
            }

            // Format
            range = ws.Cells[0, startCol, StartingDataRow - 1, col];
            FormatAsTitle(range);
        }

        private static void FormatAsTitle(IRange range)
        {
            range.HorizontalAlignment = HAlign.Center;
            range.Font.Bold = true;
            range.WrapText = true;
        }

        public void AddPracticeIndicatorData(Dictionary<string, CoreDataSet> map)
        {
            IRange cells = PracticeDataWorksheet.Cells;
            int row = StartingDataRow;
            foreach (string code in practiceCodes)
            {
                if (map.ContainsKey(code))
                {
                    AddValue(map, code, cells, row, practiceDataColumn);
                }
                row++;
            }

            practiceDataColumn += ColumnsPerIndicator; 
        }

        public void AddPracticeIndicatorValues(Dictionary<string, CoreDataSet> map)
        {
            IRange cells = PracticeDataWorksheet.Cells;
            int row = StartingDataRow;
            foreach (string code in practiceCodes)
            {
                if (map.ContainsKey(code))
                {
                    ValueWithCIsData data = map[code];
                    cells[row, practiceDataColumn].Value = Round(data.Value);
                }
                row++;
            }

            practiceDataColumn++;
        }

        public void AddAreaIndicatorData(Dictionary<string, CoreDataSet> map)
        {
            if (map != null)
            {
                IRange cells = parentAreaDataWorksheet.Cells;
                int row = StartingDataRow;
                if (map.Count > 0)
                {
                    foreach (string code in parentAreaCodes)
                    {
                        if (map.ContainsKey(code))
                        {
                            AddValue(map, code, cells, row, parentAreaDataColumn);
                        }
                        row++;
                    }
                }
            }

            parentAreaDataColumn += ColumnsPerIndicator;
        }

        public void AddCcgPopulationValues(IEnumerable<QuinaryPopulation> populations)
        {
            IRange cells = parentAreaDataWorksheet.Cells;
            int row = ccgPopulationDataRow;
            int column = parentAreaDataColumn;

            foreach (var quinaryPopulation in populations)
            {
                IList<double> vals = new QuinaryPopulationSorter(quinaryPopulation.Values).SortedValues;

                foreach (var val in vals)
                {
                    // Round to integer, populations should be atomic
                    cells[row, column++].Value = Math.Round(val, 0, MidpointRounding.AwayFromZero);
                }
            }

            ccgPopulationDataRow++;
        }

        private static void AddValue(Dictionary<string, CoreDataSet> map, string code, IRange cells, int row, int column)
        {
            CoreDataSet data = map[code];
            cells[row, column].Value = Round(data.Value);
            if (data.Are95CIsValid)
            {                
                cells[row, column + 1].Value = data.LowerCI95.HasValue ? Round(data.LowerCI95.Value).ToString() : string.Empty;
                cells[row, column + 2].Value = data.UpperCI95.HasValue ? Round(data.UpperCI95.Value).ToString() : string.Empty;
                cells[row, column + 3].Value = data.Count;
                cells[row, column + 4].Value = data.Denominator;
            }
        }

        private static double Round(double d)
        {
            return Math.Round(d, 3, MidpointRounding.AwayFromZero);
        }

        private static void AddDataHeader(IWorksheet ws, IEnumerable<ColumnHeader> headers)
        {
            const int rowIndex = 0;
            IRange row = ws.Cells[rowIndex, 0, rowIndex, headers.Count()];
            int col = 0;
            foreach (var columnHeader in headers)
            {
                SetColumnAsText(row[rowIndex, col++], columnHeader.Title);
            }

            row.Font.Bold = true;
            row.WrapText = true;

            SetColumnWidths(row, (from h in headers select h.Width).ToArray());
        }
    }
}
