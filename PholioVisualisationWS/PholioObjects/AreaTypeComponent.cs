namespace PholioVisualisation.PholioObjects
{
    public class AreaTypeComponent
    {
        /// <summary>
        /// The area type that is comprised of more than one other area types
        /// </summary>
        public int AreaTypeId { get; set; }

        /// <summary>
        /// One of the area types that is a component of the parent
        /// </summary>
        public int ComponentAreaTypeId { get; set; }

        public override bool Equals(object obj)
        {
            AreaTypeComponent other = obj as AreaTypeComponent;

            if (other != null)
            {
                return AreaTypeId == other.AreaTypeId &&
                       ComponentAreaTypeId == other.ComponentAreaTypeId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (AreaTypeId + "-" + ComponentAreaTypeId).GetHashCode();
        }
    }
}