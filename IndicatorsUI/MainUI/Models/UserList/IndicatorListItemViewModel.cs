using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models.UserList
{
    public class IndicatorListItemViewModel
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public int IndicatorId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
    }
}