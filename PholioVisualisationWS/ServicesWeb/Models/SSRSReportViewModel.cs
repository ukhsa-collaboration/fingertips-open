using System.Collections.Generic;

namespace ServicesWeb.Models
{
    public class SSRSReportViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Parameter { get; set; }
    }
}
