
namespace PholioVisualisation.PholioObjects
{
    public class AreaHierarchy
    {
        public string ParentAreaCode { get; set; }
        public string ChildAreaCode { get; set; }

        public override bool Equals(object obj)
        {
            AreaHierarchy other = obj as AreaHierarchy;

            if (other != null)
            {
                return ParentAreaCode == other.ParentAreaCode &&
                    ChildAreaCode == other.ChildAreaCode;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (ParentAreaCode + ChildAreaCode).GetHashCode();
        }

    }
}
