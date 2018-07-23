namespace Fpm.MainUI.Models
{
    public class CopyIndicatorsModel : MoveIndicatorsModel
    {
        /// <summary>
        /// The Group ID to copy from.
        /// </summary>
        public int GroupId { get; set; }

        public int TargetAreaTypeId { get; set; }
        public int TargetGroupId { get; set; }
        public string TargetProfileUrlKey { get; set; }
    }
}