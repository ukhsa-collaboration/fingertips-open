using System;

namespace Fpm.ProfileData.Entities.Logging
{
    public class DatabaseLog
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public DateTime Timestamp { get; set; }
    }
}