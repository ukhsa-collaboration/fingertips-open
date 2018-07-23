using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public struct SignificanceLabel
    {
        public Significance Significance;
        public string Label;

        public static SignificanceLabel New(Significance significance, string label)
        {
            return new SignificanceLabel
            {
                Significance = significance,
                Label = label
            };
        }
    }

}