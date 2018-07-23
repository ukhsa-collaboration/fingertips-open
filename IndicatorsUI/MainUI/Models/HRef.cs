using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models
{
    public class HRef
    {
        public string Start { private get; set; }
        public string End { private get; set; }

        public string Join(string middle)
        {
            return Start + middle + End;
        }
    }
}