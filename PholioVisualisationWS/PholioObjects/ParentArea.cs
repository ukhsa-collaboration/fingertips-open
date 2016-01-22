namespace PholioVisualisation.PholioObjects
{
    public class ParentArea
    {
        public string AreaCode { get; set; }
        public int ChildAreaTypeId { get; set; }

        public ParentArea(string parentAreaCode, int childAreaTypeId)
        {
            AreaCode = parentAreaCode;
            ChildAreaTypeId = childAreaTypeId;
        }

        public override bool Equals(object obj)
        {
            ParentArea other = obj as ParentArea;

            if (other != null)
            {
                return AreaCode == other.AreaCode &&
                       ChildAreaTypeId == other.ChildAreaTypeId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (AreaCode + "-" + ChildAreaTypeId).GetHashCode();
        }

    }
}
