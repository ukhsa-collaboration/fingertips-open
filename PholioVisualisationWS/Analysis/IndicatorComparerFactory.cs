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

        private IndicatorComparer _comparer;
        private bool _assignConfidenceVariable;
        private Grouping _grouping;

        public IndicatorComparer New(Grouping grouping)
        {
            _assignConfidenceVariable = false;
            _grouping = grouping;

            CheckDependencies();

            switch (grouping.ComparatorMethodId)
            {
                case ComparatorMethodIds.NoComparison:
                    _comparer = new NoComparisonComparer();
                    break;
                case ComparatorMethodIds.SingleOverlappingCIs:
                    _comparer = new SingleOverlappingCIsComparer();
                    break;
                case ComparatorMethodIds.SpcForProportions:
                    _comparer = new SpcForProportionsComparer();
                    _assignConfidenceVariable = true;
                    break;
                case ComparatorMethodIds.SpcForDsr:
                    _comparer = new SpcForDsrComparer();
                    _assignConfidenceVariable = true;
                    break;
                case ComparatorMethodIds.DoubleOverlappingCIs:
                    _comparer = new DoubleOverlappingCIsComparer();
                    break;
                case ComparatorMethodIds.Quintiles:
                    _comparer = new QuintilesComparer();
                    break;
                case ComparatorMethodIds.Quartiles:
                    _comparer = new QuartilesComparer();
                    break;
                case ComparatorMethodIds.SuicidePreventionPlan:
                    _comparer = new SuicidePreventPlanComparer();
                    break;
                default:
                    throw new FingertipsException("Invalid comparator method ID: " + grouping.ComparatorMethodId);
            }

            AssignConfidenceVariable();

            _comparer.PolarityId = grouping.PolarityId;

            return _comparer;
        }

        private void AssignConfidenceVariable()
        {
            if (_assignConfidenceVariable)
            {
                var comparatorConfidence = PholioReader.GetComparatorConfidence
                    (_grouping.ComparatorMethodId, _grouping.ComparatorConfidence);

                _comparer.ConfidenceVariable = comparatorConfidence.ConfidenceVariable;
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
