using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PholioVisualisation.UserData.IndicatorLists
{
    public class IndicatorListItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("listid")]
        public int ListId { get; set; }

        public IndicatorList IndicatorList { get; set; }

        public string IndicatorName;

        [Column("indicatorid")]
        public int IndicatorId { get; set; }

        [Column("sexid")]
        public int SexId { get; set; }

        [Column("ageid")]
        public int AgeId { get; set; }

        [Column("sequence")]
        public int Sequence { get; set; }

        public Sex Sex { get; set; }
        public Age Age { get; set; }
    }
}
