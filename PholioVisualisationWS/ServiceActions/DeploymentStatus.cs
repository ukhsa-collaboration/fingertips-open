using System;

namespace PholioVisualisation.ServiceActions
{
    public class DeploymentStatus
    {
        public string WebSite { get; set; }
        public string DataFiles { get; set; }
        public string Database { get; set; }
        public string BuildVersion { get; set; }
        public DateTime DatabaseBackUpTimestamp { get; set; }
    }
}