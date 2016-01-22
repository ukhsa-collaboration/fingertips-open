
namespace PholioVisualisation.PholioObjects
{
    public class AreaCodeNeighbourMapping
    {
        public int Id { get; set; }
        public string AreaCode { get; set; }
        public int Position { get; set; }
        public string NeighbourAreaCode { get; set; }
        public int NeighbourTypeId { get; set; }
    }
}
