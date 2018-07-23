using System;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUI.Helpers
{
    public class SpineChartMinMaxLabelBuilder
    {
        private readonly SpineChartMinMaxLabel _sourceLabel;
        private int _keyColor;

        public class MinMaxLabel
        {
            public int LabelId { get; set; }
            public string Min { get; set; }
            public string Max { get; set; }
        }

        public MinMaxLabel MinMaxLabels
        {
           get {
               if (_sourceLabel.Id == SpineChartMinMaxLabels.WorstAndBest)
               {
                   _keyColor = KeyColours.RagOnly;
               }

               if (_sourceLabel.Id == SpineChartMinMaxLabels.LowestAndHighest)
               {
                   _keyColor = KeyColours.BluesOnly;
               }

               if (_sourceLabel.Id == SpineChartMinMaxLabels.WorstLowestAndBestHighest)
               {
                   _keyColor = KeyColours.RagAndBlues;
               }

               return GetMinMaxLabelFromKeyColour();
           }
        }

        public SpineChartMinMaxLabelBuilder(SpineChartMinMaxLabel label)
        {
            if (label == null)
            {
                throw new ArgumentNullException();
            }
            this._sourceLabel = label;
        }

        public SpineChartMinMaxLabelBuilder(SpineChartMinMaxLabel label, int keyColor) 
            :this(label)
        {
            this._keyColor = keyColor;
        }
    
        private MinMaxLabel GetMinMaxLabelFromKeyColour()
        {

            if (_keyColor == KeyColours.RagOnly)
            {
                return new MinMaxLabel()
                {
                    LabelId = SpineChartMinMaxLabels.WorstAndBest,
                    Min = "Worst",
                    Max = "Best"
                };
            }

            if (_keyColor == KeyColours.BluesOnly)
            {

                return new MinMaxLabel()
                {
                    LabelId = SpineChartMinMaxLabels.LowestAndHighest,
                    Min = "Lowest",
                    Max = "Highest"
                };
            }

            return DefaultMinMaxLabel();
        }

        private MinMaxLabel DefaultMinMaxLabel()
        {
            return new MinMaxLabel
            {
                LabelId = SpineChartMinMaxLabels.WorstLowestAndBestHighest,
                Min = "Worst/ Lowest",
                Max = "Best/ Highest"
            }; 
        }
    }
}