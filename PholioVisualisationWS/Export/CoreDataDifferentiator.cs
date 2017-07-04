namespace PholioVisualisation.Export
{
    /// <summary>
    /// Properties that allow data objects with the same time period
    /// to be differentiated from each other
    /// </summary>
    public class CoreDataDifferentiator
    {
        public string AreaCode { get; set; }
        public int CategoryId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
    }
}