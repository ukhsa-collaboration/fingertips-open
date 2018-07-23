using System;

namespace Fpm.ProfileData.Entities.UserFeedback
{
    public class UserFeedback
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string WhatUserWasDoing { get; set; }
        public string WhatWentWrong { get; set; }
        public string Email { get; set; }
        public string Environment { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasBeenDealtWith { get; set; }
    }
}