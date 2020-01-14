namespace PholioVisualisation.Export.FileBuilder.SupportModels
{
    public class GroupingInequality
    {
        public int IndicatorId { get; set; }
        public int AreaTypeId { get; set; }
        public int? GroupId { get; set; }
        public int Sex { get; set; }
        public int Age { get; set; }

        public GroupingInequality(int indicatorId, int areaTypeId, int? groupId, int sex, int age)
        {
            IndicatorId = indicatorId;
            AreaTypeId = areaTypeId;
            GroupId = groupId;
            Sex = sex;
            Age = age;
        }
    }
}
