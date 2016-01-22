namespace PholioVisualisation.PdfData
{
    /// <summary>
    ///     Data required for one of the life expectancy charts at the
    ///     bottom of page 2 of the Health Profiles
    /// </summary>
    public class LifeExpectancyChartData
    {
        public XyPoint[] Deciles { get; set; }
        public XyPoint[] LineOfBestFit { get; set; }
    }
}