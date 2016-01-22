using System.Collections.Generic;
using Fpm.ProfileData.Entities.Core;

namespace Fpm.MainUI.ViewModels
{
    public class CoreDataSetViewModel
    {
        public int RowsFound { get; set; }

        public IEnumerable<CoreDataSet> DataSet { get; set; }

        public bool? DeleteStatus { get; set; }

        public string Message { get; set; }

        public bool CanDeleteDataSet { get; set; }
    }
}