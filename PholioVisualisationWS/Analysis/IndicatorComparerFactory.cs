using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class IndicatorComparerFactory
    {
        public PholioReader PholioReader { get; set; }

        private IndicatorComparer comparer;
        private bool assignConfidenceVariable;
        private Grouping grouping;

        public IndicatorComparer New(Grouping grouping)
        {
            assignConfidenceVariable = false;
            this.grouping = grouping;

            CheckDependencies();

            switch (grouping.ComparatorMethodId)
            {
                case ComparatorMethodId.NoComparison:
                    comparer = new NoComparisonComparer();
                    break;
                case ComparatorMethodId.SingleOverlappingCIs:
                    comparer = new SingleOverlappingCIsComparer();
                    break;
                case ComparatorMethodId.SpcForProportions:
                    comparer = new SpcForProportionsComparer();
                    assignConfidenceVariable = true;
                    break;
                case ComparatorMethodId.SpcForDsr:
                    comparer = new SpcForDsrComparer();
                    assignConfidenceVariable = true;
                    break;
                case ComparatorMethodId.DoubleOverlappingCIs:
                    comparer = new DoubleOverlappingCIsComparer();
                    break;
                case ComparatorMethodId.Quintiles:
                    comparer = new QuintilesComparer();
                    break;
                case ComparatorMethodId.SuicidePreventionPlan:
                    comparer = new SuicidePreventPlanComparer();
                    break;
                default:
                    throw new FingertipsException("Invalid comparator method ID: " + grouping.ComparatorMethodId);
            }

            AssignConfidenceVariable();

            comparer.PolarityId = grouping.PolarityId;

            return comparer;
        }

        private void AssignConfidenceVariable()
        {
            if (assignConfidenceVariable)
            {
                var comparatorConfidence = PholioReader.GetComparatorConfidence
                    (grouping.ComparatorMethodId, grouping.ComparatorConfidence);

                comparer.ConfidenceVariable = comparatorConfidence.ConfidenceVariable;
            }
        }

        private void CheckDependencies()
        {
            if (PholioReader == null)
            {
                throw new FingertipsException("PholioReader not set");
            }
        }
    }
}
