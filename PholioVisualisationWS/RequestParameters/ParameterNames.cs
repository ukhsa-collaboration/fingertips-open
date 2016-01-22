
namespace PholioVisualisation.RequestParameters
{
    /// <summary>
    /// Standard parameter names.
    /// </summary>
    public class ParameterNames
    {
        public const string ProfileId = "pid";
        public const string ProfileIdFull = "profile_id";
        public const string RestrictToProfileId = "res";
        public const string TemplateProfileId = "tem";

        public const string Ids = "ids";
        public const string GroupIds = "gid";
        public const string AreaTypeId = "ati";
        public const string AreaCode = "are";
        public const string AreaCategory = "act";
        public const string ParentAreaCode = "par";
        public const string NearestNeighbourCode = "nn";
        public const string ParentAreaTypeId = "pat";
        public const string PolygonAreaTypeId = "polygon_area_type_id";
        public const string CategoryTypeId = "category_type_id";
        public const string AreEastingAndNorthingRequired = "include_coordinates";
        public const string ExcludeCcGs = "exclude_ccgs";
        public const string Text = "text";
        public const string IgnoredAreas = "ign"; // DO NOT USE - read codes from PHOLIO instead
        public const string AreaCodes = "acs";
        public const string Name = "nam";
        public const string ParentsToDisplay = "pds";
        public const string Key = "key";
        public const string PracticeYear = "pyr";
        public const string IndicatorId = "iid";
        public const string SexId = "sex";
        public const string AgeId = "age";
        public const string Type = "typ";
        public const string JsonpCallback = "callback";

        public const string Quarter = "qtr";
        public const string Month = "month";
        public const string Year = "yr";
        public const string YearRange = "yrr";
        public const string YearTypeId = "yti";
        public const string ComparatorId = "com";

        public const string RetrieveIgnoredAreas = "ria";

        //Exception Logging Parameters
        public const string Application = "app";
        public const string UserName = "unm";
        public const string ErrorMessage = "msg";
        public const string StackTrace = "stc";
        public const string ExceptionType = "ext";
        public const string Url = "ur";
        public const string Environment = "env";
        public const string Server = "svr";
   
    }
}
