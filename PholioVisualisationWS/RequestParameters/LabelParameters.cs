
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PholioVisualisation.RequestParameters
{
    public class LabelParameters : BaseParameters
    {
        public const string ParameterAjaxKey = "key";
        public const string ParameterComparatorMethod = "cm";
        public const string ParameterConfidenceIntervalMethod = "cim";

        public int AjaxKey { get; set; }
        public int AgeId { get; set; }
        public int YearTypeId { get; set; }
        public int ComparatorMethodId { get; set; }
        public int ConfidenceIntervalMethodId { get; set; }

        public LabelParameters(NameValueCollection parameters)
            : base(parameters)
        {
            AgeId = ParseInt(ParameterNames.AgeId);
            YearTypeId = ParseInt(ParameterNames.YearTypeId);
            AjaxKey = ParseInt(ParameterAjaxKey);
            ComparatorMethodId = ParseInt(ParameterComparatorMethod);
            ConfidenceIntervalMethodId = ParseInt(ParameterConfidenceIntervalMethod);
        }

        public bool IsAgeIdValid
        {
            get { return AgeId > 0; }
        }

        public bool IsYearTypeIdValid
        {
            get { return YearTypeId > 0; }
        }

        public bool IsComparatorMethodIdValid
        {
            get { return ComparatorMethodId > 0; }
        }

        public bool IsConfidenceIntervalMethodIdValid
        {
            get { return ConfidenceIntervalMethodId > 0; }
        }

        public override bool AreValid
        {
            get
            {
                return AjaxKey > 0 &&
                    (IsAgeIdValid ||
                    IsYearTypeIdValid ||
                    IsComparatorMethodIdValid ||
                    IsConfidenceIntervalMethodIdValid);
            }
        }
    }
}
