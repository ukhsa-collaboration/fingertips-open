using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PholioVisualisation.UserData.IndicatorLists
{
    public interface IIndicatorListRepository
    {
        DbSet<IndicatorList> IndicatorLists { get; set; }
        IndicatorList GetIndicatorList(string publicId);
    }
}
