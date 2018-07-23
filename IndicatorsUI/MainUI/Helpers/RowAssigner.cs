namespace IndicatorsUI.MainUI.Helpers
{
    /// <summary>
    ///     Assigns a collection of items into a grid layout. Assignment
    ///     proceeds by filling each row in turn.
    /// </summary>
    public class RowAssigner
    {
        private readonly int columnCount;
        private int indexOfCurrentItem;

        public RowAssigner(int columnCount)
        {
            this.columnCount = columnCount;
        }

        public bool IsItemFirstOfRow
        {
            get { return (indexOfCurrentItem%columnCount) == 0; }
        }

        public bool IsItemLastOfRow
        {
            get { return (indexOfCurrentItem%columnCount) == columnCount - 1; }
        }

        public void ItemAdded()
        {
            indexOfCurrentItem++;
        }
    }
}