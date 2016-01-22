using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public abstract class AverageCalculator
    {
        public abstract CoreDataSet Average
        {
            get;
        }

    }
}
