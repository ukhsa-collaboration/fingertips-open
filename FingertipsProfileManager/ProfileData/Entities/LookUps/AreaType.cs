using System.ComponentModel;

namespace Fpm.ProfileData.Entities.LookUps
{
    public class AreaType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Short name")]
        public string ShortName { get; set; }

        public bool IsCurrent { get; set; }

        [DisplayName("Selectable")]
        public bool IsSupported { get; set; }

        [DisplayName("Searchable")]
        public bool IsSearchable { get; set; }

        [DisplayName("Warn for small numbers")]
        public bool ShouldWarnForSmallNumbers { get; set; }
    }
}
