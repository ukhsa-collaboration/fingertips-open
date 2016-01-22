
using SpreadsheetGear;

namespace PholioVisualisation.Export
{
    public class WorksheetInfo
    {
        public IWorksheet Worksheet { get; set; }

        private int row;

        /// <summary>
        /// Returns the index of the next row. The row number is incremented on each call.
        /// </summary>
        public int NextRow
        {
            get
            {
                return row++;
            }
        }

        /// <summary>
        /// A worksheet is considered empty if NextRow has only been called 0 or 1 times.
        /// </summary>
        public bool IsWorksheetEmpty
        {
            get
            {
                return row < 2;
            }
        }
    }
}
