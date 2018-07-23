using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Models
{
    public class IndicatorEdit : BaseDataModel
    {
        public IList<IndicatorText> TextValues { get; set; }
        public int SelectedIndicatorId { get; set; }

        public bool DoesIndicatorDataExist()
        {
            return TextValues.Count > 0;
        }

        public bool AreAnySpecificTextValues()
        {
            return TextValues.Any(x => x.HasSpecificValue());
        }

        public int IndicatorIdNext { get; set; }
        public int IndicatorIdPrevious { get; set; }
        public string urlKey { get; set; }
     
        public Grouping Grouping { get; set; }

        public string ReturnUrl { get; set; }

    }
}