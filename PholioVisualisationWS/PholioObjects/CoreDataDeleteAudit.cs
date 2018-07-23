using System;

namespace PholioVisualisation.PholioObjects
{
    public class CoreDataDeleteAudit
    {
        public int Id { get; set; }
        public int IndicatorId { get; set; }
        public string WhereCondition { get; set; }
        public int RowsDeleted { get; set; }
        public DateTime DateDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string DeleteBatchId { get; set; }
    }
}
