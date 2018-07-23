using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PholioVisualisation.UserData.IndicatorLists
{
    public class IndicatorList
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("publicid")]
        public string PublicId { get; set; }

        [Column("listname")]
        public string Name { get; set; }

        public ICollection<IndicatorListItem> IndicatorListItems { get; set; }

        public void OrderListItemsBySequence()
        {
            IndicatorListItems = IndicatorListItems.OrderBy(x => x.Sequence).ToList();
        }

    }
}