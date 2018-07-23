using System;

namespace PholioVisualisation.PholioObjects
{
    public class DatabaseLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }
}