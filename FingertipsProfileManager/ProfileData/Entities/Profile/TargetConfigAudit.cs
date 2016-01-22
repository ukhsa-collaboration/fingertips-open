using System;

namespace Fpm.ProfileData.Entities.Profile
{
    public class TargetConfigAudit
    {
        /// <summary>
        /// Required by NHibernate
        /// </summary>
        public TargetConfigAudit() { }

        public TargetConfigAudit(TargetConfig target, int userId, string change)
        {
            // Set TargetConfig properties
            Id = target.Id;
            Description = target.Description;
            LowerLimit = target.LowerLimit;
            UpperLimit = target.UpperLimit;
            PolarityId = target.PolarityId;
            BespokeTargetKey = target.BespokeTargetKey;

            // Set audit properties
            Timestamp = DateTime.Now;
            UserId = userId;
            Change = change;
        }

        // TargetConfig properties
        public int Id { get; set; }
        public double? LowerLimit { get; set; }
        public double? UpperLimit { get; set; }
        public string BespokeTargetKey { get; set; }
        public int PolarityId { get; set; }
        public string Description { get; set; }

        // Audit properties
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Change { get; set; }
        public DateTime Timestamp { get; set; }
    }
}