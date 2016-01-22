
namespace PholioVisualisation.PholioObjects
{
    public class CategorisedArea
    {
        public int Id { get; set; }
        public string AreaCode { get; set; }
        public int ParentAreaTypeId { get; set; }
        public int ChildAreaTypeId { get; set; }
        public int CategoryTypeId { get; set; }
        public int CategoryId { get; set; }
    }
}
