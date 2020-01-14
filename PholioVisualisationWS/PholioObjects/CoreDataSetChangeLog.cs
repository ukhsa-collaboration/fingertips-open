using System;

namespace PholioVisualisation.PholioObjects
{
    public class CoreDataSetChangeLog
    {
        public int Id { get; set; }
        public int IndicatorId { get; set; }
        public int AreaTypeId { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
