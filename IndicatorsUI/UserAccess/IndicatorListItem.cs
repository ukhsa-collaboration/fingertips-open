//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IndicatorsUI.UserAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class IndicatorListItem
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public int IndicatorId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public int Sequence { get; set; }
    
        public virtual IndicatorList IndicatorList { get; set; }
    }
}