using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public class TrendMarkerLabelProvider
    {
        private readonly Dictionary<TrendMarker, TrendMarkerLabel> _trendMarkerToLabel;
        public readonly IList<TrendMarkerLabel> Labels;

        public TrendMarkerLabelProvider(int polarityId)
        {
            switch (polarityId)
            {
                case PolarityIds.RagLowIsGood:
                    Labels = new List<TrendMarkerLabel> {
                        CannotBeCalculated,
                        new TrendMarkerLabel{Id = TrendMarker.Increasing, Text = "Increasing and getting worse"},
                        new TrendMarkerLabel{Id = TrendMarker.Decreasing, Text = "Decreasing and getting better"},
                        NoChange
                    };
                    break;

                case PolarityIds.RagHighIsGood:
                    Labels = new List<TrendMarkerLabel> {
                        CannotBeCalculated,
                        new TrendMarkerLabel{Id = TrendMarker.Increasing, Text = "Increasing and getting better"},
                        new TrendMarkerLabel{Id = TrendMarker.Decreasing, Text = "Decreasing and getting worse"},
                        NoChange
                    };
                    break;

                default:
                    Labels = new List<TrendMarkerLabel> {
                        CannotBeCalculated,
                        new TrendMarkerLabel{Id = TrendMarker.Increasing, Text = "Increasing"},
                        new TrendMarkerLabel{Id = TrendMarker.Decreasing, Text = "Decreasing"},
                        NoChange
                    };
                    break;
            }

            _trendMarkerToLabel = Labels.ToDictionary(x => x.Id, x => x);
        }

        public TrendMarkerLabel GetLabel(TrendMarker trendMarker)
        {
            return _trendMarkerToLabel[trendMarker];
        }

        private TrendMarkerLabel CannotBeCalculated
        {
            get
            {
                return new TrendMarkerLabel
                {
                    Id = TrendMarker.CannotBeCalculated, Text = "Cannot be calculated"
                };
            }
        }

        private TrendMarkerLabel NoChange
        {
            get
            {
                return new TrendMarkerLabel
                {
                    Id = TrendMarker.NoChange, Text = "No significant change"
                };
            }
        }
    }
}