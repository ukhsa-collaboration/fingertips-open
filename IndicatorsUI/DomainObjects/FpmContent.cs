using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiles.DomainObjects
{
    public class FpmContent
    {
        public int Id { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
