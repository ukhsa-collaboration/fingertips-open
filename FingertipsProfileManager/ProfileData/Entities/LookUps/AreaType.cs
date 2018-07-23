using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        public ICollection<AreaTypeComponent> ComponentAreaTypes { get; set; }

        [DisplayName("Component area types")]
        public string ComponentAreaTypesString { get; set; }

        public AreaType()
        {
            ComponentAreaTypes = new List<AreaTypeComponent>();
        }

        public string GetComponentAreaTypesAsString()
        {
            return string.Join(",", ComponentAreaTypes.Select(x => x.ComponentAreaTypeId));
        }

        public void ReplaceComponentAreaTypes(ICollection<AreaTypeComponent> areaTypeComponents)
        {
            var toDelete = new List<AreaTypeComponent>();
            var toKeep = new List<AreaTypeComponent>();

            // Process current components
            foreach (var existingAreaTypeComponent in ComponentAreaTypes)
            {
                if (areaTypeComponents.Contains(existingAreaTypeComponent))
                {
                    toKeep.Add(existingAreaTypeComponent);
                }
                else
                {
                    toDelete.Add(existingAreaTypeComponent);
                }
            }

            // Keep new components
            foreach (var newAreaTypeComponent in areaTypeComponents)
            {
                if (toKeep.Contains(newAreaTypeComponent) == false)
                {
                    toKeep.Add(newAreaTypeComponent);
                }
            }

            ComponentAreaTypes = toKeep;
            ComponentAreaTypesToDelete = toDelete;
        }

        public ICollection<AreaTypeComponent> ComponentAreaTypesToDelete { get; set; }

        /// <summary>
        /// Convert ComponentAreaTypesString to ComponentAreaTypes
        /// </summary>
        public void ParseComponentAreaTypesString()
        {
            var newComponents = new List<AreaTypeComponent>();
            if (ComponentAreaTypesString != null)
            {
                var splits = ComponentAreaTypesString.Split(',');
                for (int i = 0; i < splits.Length; i++)
                {
                    int childAreaTypeId;
                    if (int.TryParse(splits[i].Trim(), out childAreaTypeId))
                    {
                        newComponents.Add(new AreaTypeComponent
                        {
                            AreaTypeId = Id,
                            ComponentAreaTypeId = childAreaTypeId
                        });
                    }
                }
            }
            ReplaceComponentAreaTypes(newComponents);
        }

    }
}
