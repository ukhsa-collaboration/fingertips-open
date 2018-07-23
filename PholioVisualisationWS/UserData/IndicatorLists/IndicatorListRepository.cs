using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace PholioVisualisation.UserData.IndicatorLists
{
    public class IndicatorListRepository : DbContext, IIndicatorListRepository
    {
        public IndicatorListRepository()
        : base("FingertipsUsersConnectionstring")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<IndicatorListRepository>(null);

            modelBuilder.Entity<IndicatorList>()
                .ToTable("IndicatorList")
                .HasMany(t => t.IndicatorListItems)
                .WithRequired()
                .HasForeignKey(x => x.ListId);

            modelBuilder.Entity<IndicatorListItem>()
                .ToTable("IndicatorListItem")
                .HasRequired(t => t.IndicatorList)
                .WithMany(x => x.IndicatorListItems)
                .HasForeignKey(x => x.ListId);

            base.OnModelCreating(modelBuilder);
        }

        public IndicatorList GetIndicatorList(string publicId)
        {
            var list = IndicatorLists.Include("IndicatorListItems")
                .FirstOrDefault(x => x.PublicId == publicId);

            if (list != null)
            {
                list.OrderListItemsBySequence();
            }

            return list;
        }

        public IList<int> GetIndicatorIdsInIndicatorList(string publicId)
        {
            var list = GetIndicatorList(publicId);
            if (list == null) return new List<int>();

            return list.IndicatorListItems
                .Select(x => x.IndicatorId)
                .ToList();
        }

        public DbSet<IndicatorList> IndicatorLists { get; set; }
    }
}
