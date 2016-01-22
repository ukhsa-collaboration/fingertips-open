namespace Fpm.ProfileData.Entities.Profile
{
    public class Category
    {
        public int CategoryTypeId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public override bool Equals(object obj)
        {
            Category other = obj as Category;
            if (other != null)
            {
                return CategoryTypeId == other.CategoryTypeId && CategoryId == other.CategoryId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (CategoryTypeId.ToString() + CategoryId.ToString()).GetHashCode();
        }
    }
}
