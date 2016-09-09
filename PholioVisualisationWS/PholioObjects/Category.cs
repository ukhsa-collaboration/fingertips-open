
namespace PholioVisualisation.PholioObjects
{
    public class Category : INamedEntity
    {
        public int Id { get; set; }
        public int CategoryTypeId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public override bool Equals(object obj)
        {
            Category other = obj as Category;

            if (other != null)
            {
                return Id == other.Id &&
                       CategoryTypeId == other.CategoryTypeId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (CategoryTypeId + "-" + Id).GetHashCode();
        }
    }
}
