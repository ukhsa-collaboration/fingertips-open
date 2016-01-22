
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.DataConstruction
{
    public interface ICacheable
    {
        bool CanBeCached { get; }
    }
}
