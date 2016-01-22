using System;

namespace PholioVisualisation.PholioObjects
{
    public class ParentAreaGroup
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int ChildAreaTypeId { get; set; }
        public int? ParentAreaTypeId { get; set; }
        public int? CategoryTypeId { get; set; }
        public int Sequence { get; set; }
    }
}
