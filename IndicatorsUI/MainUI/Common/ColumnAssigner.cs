using System;

namespace Profiles.MainUI.Common
{
    /// <summary>
    /// Assigns a collection of items into a specified number of columns. Assignment
    /// proceeds by filling each column in turn.
    /// </summary>
    public class ColumnAssigner
    {
        private int index;

        private double itemsPerColumn;
        private int itemsInColumnLimit;
        private int itemsBeforeNextColumn;
        private int itemCount;

        public ColumnAssigner(int itemCount, int columnCount)
        {
            this.itemCount = itemCount;

            itemsPerColumn = Convert.ToDouble(itemCount) /
                Convert.ToDouble(columnCount);

            itemsInColumnLimit = (int)Math.Ceiling(itemsPerColumn);

            itemsBeforeNextColumn = itemsInColumnLimit;
        }

        public bool IsNextIndexInCurrentColumn
        {
            get
            {
                return index < itemsBeforeNextColumn &&
                    index < itemCount;
            }
        }

        public void NewColumn()
        {
            itemsBeforeNextColumn += itemsInColumnLimit;

        }

        public int NextIndex
        {
            get
            {
                return index++;
            }
        }

    }
}