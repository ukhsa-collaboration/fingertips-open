using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models
{
    public class PageOption
    {
        public PageOption(string label, string cssClass, string id, string javaScript)
        {
            Id = id;
            Label = label;
            JavaScript = javaScript;
            IconCssClass = cssClass;
        }
        public string Id { get; set; }
        public string Label { get; set; }
        public string JavaScript { get; set; }
        public string IconCssClass { get; set; }
        public string TabCssClass { get; set; }
        public bool IsSelected { get; set; }

    }
}