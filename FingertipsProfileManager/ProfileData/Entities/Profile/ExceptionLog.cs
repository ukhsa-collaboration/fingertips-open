using System;

namespace Fpm.ProfileData.Entities.Profile
{
    public class ExceptionLog
    {
        public int Id { get; set; }

        public string Application { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Server { get; set; }
    }
}
