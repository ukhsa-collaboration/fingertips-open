
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Group> Children { get; set; }  
    }
}
