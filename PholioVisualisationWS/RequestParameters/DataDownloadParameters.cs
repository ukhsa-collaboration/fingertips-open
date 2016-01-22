
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.RequestParameters
{
    public class DataDownloadParameters : DataParameters
    {
        public List<int> RestrictResultsToProfileIdList { get; set; }

        public ParentDisplay ParentsToDisplay { get; set; }
        public int TemplateProfileId { get; set; }
        public int AreaTypeId { get; set; }
        public string ParentAreaCode { get; set; }
        public int SubnationalAreaTypeId { get; set; }

        public DataDownloadParameters(NameValueCollection parameters)
            : base(parameters)
        {
            // Parents to include in the data download
            int enumValue = ParseInt(ParameterNames.ParentsToDisplay);
            ParentsToDisplay = IsParentDisplayValid(enumValue) ?
                (ParentDisplay)enumValue :
                ParentDisplay.Undefined;

            TemplateProfileId = ParseInt(ParameterNames.TemplateProfileId);
            AreaTypeId = ParseInt(ParameterNames.AreaTypeId);
            ParentAreaCode = ParseString(ParameterNames.ParentAreaCode);
            SubnationalAreaTypeId = ParseInt(ParameterNames.ParentAreaTypeId);
            RestrictResultsToProfileIdList = ParseIntList(ParameterNames.RestrictToProfileId, Convert.ToChar(","));
        }

        public static bool IsParentDisplayValid(int enumValue)
        {
            return
                enumValue == (int)ParentDisplay.NationalAndRegional ||
                enumValue == (int)ParentDisplay.NationalOnly ||
                enumValue == (int)ParentDisplay.RegionalOnly;
        }

        public override bool AreValid
        {
            get
            {
                return (ProfileId > 0 || IndicatorIds.Count > 0) &&
                    string.IsNullOrEmpty(ParentAreaCode) == false &&
                    AreaTypeId != UndefinedInteger &&
                    ParentsToDisplay != ParentDisplay.Undefined;
            }
        }
    }
}
