using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class CategoryComparisonDifferentiator
    {
        public readonly int AgeId;
        public readonly int SexId;
        public readonly string ParentCode;

        public CategoryComparisonDifferentiator(CoreDataSet coreDataSet, string parentCode)
        {
            AgeId = coreDataSet.AgeId;
            SexId = coreDataSet.SexId;
            ParentCode = parentCode;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return AgeId * 10 + SexId + ParentCode.GetHashCode();
        }
    }
}