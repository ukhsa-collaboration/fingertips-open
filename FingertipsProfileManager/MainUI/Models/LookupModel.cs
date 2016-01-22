using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class LookupModel
    {
        public string LookupType { get; set; }

        public IList<Sex> Sexes { get; set; }
        public IList<Age> Ages { get; set; }
        public IList<ValueNote> ValueNotes { get; set; }
        public IList<CategoryType> CategoryTypes { get; set; }
        public int CategoryTypeId { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<TargetConfig> Targets { get; set; }
        public IList<Polarity> Polarities { get; set; }

        public CategoryType GetSelectedCategoryType()
        {
            return CategoryTypes.First(x => x.Id == CategoryTypeId);
        }

        public string GetPolarityNameFromId(int polarityId)
        {
            return Polarities.First(x => x.Id == polarityId).Name;
        }
    }
}