using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class ChartParameters : BaseParameters
    {
        public const string ParameterWidth = "width";
        public const string ParameterHeight = "height";
        public const int DefaultDimension = 300;
        public const int MaxDimension = 1000;

        public ChartParameters(NameValueCollection parameters)
            : base(parameters)
        {
            ParseHeight();
            ParseWidth();
        }

        public int Height { get; set; }
        public int Width { get; set; }

        private void ParseWidth()
        {
            string width = Parameters[ParameterWidth];
            if (string.IsNullOrEmpty(width) == false)
            {
                Width = int.Parse(width);
                if (Width < 0 || Width > MaxDimension)
                {
                    Width = DefaultDimension;
                }
            }
            else
            {
                Width = DefaultDimension;
            }
        }

        private void ParseHeight()
        {
            string height = Parameters[ParameterHeight];
            if (string.IsNullOrEmpty(height) == false)
            {
                Height = int.Parse(height);
                if (IsDimensionValid(Height) == false)
                {
                    Height = DefaultDimension;
                }
            }
            else
            {
                Height = DefaultDimension;
            }
        }

        private static bool IsDimensionValid(int dimension)
        {
            return !(dimension < 0 || dimension > MaxDimension);
        }
    }
}