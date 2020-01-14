using System;
using System.Collections.Generic;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Validations
{
    /// <summary>
    /// Interface IDataInternalServicesValidation
    /// </summary>
    public interface IRequestedParametersValidation
    {
        /// <summary>
        /// ValidateChildAreaTypeId
        /// </summary>
        /// <returns></returns>
        Exception ValidateChildAreaTypeId();

        /// <summary>
        /// ValidateParentAreaTypeId
        /// </summary>
        Exception ValidateParentAreaTypeId();
        /// <summary>
        /// ValidateIndicatorIds
        /// </summary>
        /// <returns></returns>
        Exception ValidateIndicatorIds();
        /// <summary>
        /// ValidateAreaCode
        /// </summary>
        Exception ValidateAreaCode();
        /// <summary>
        /// ValidateCategoryAreaCode
        /// </summary>
        Exception ValidateCategoryAreaCode();
        /// <summary>
        /// ValidateProfileId
        /// </summary>
        Exception ValidateProfileId();
        /// <summary>
        /// ValidateGroupId
        /// </summary>
        Exception ValidateGroupId();
        /// <summary>
        /// ValidateInequalities
        /// </summary>
        Exception ValidateInequalities();
        /// <summary>
        /// ValidateParentAreaCode
        /// </summary>
        Exception ValidateParentAreaCode();

        /// <summary>
        /// ValidateSex
        /// </summary>
        Exception ValidateSex();

        /// <summary>
        /// ValidateAge
        /// </summary>
        Exception ValidateAge();
    }

    /// <summary>
    /// Group all requested parameters from url
    /// </summary>
    public class RequestedParameters : AbstractValidator, IRequestedParametersValidation, IDataParameters
    {
        protected int ChildAreaTypeId { get; private set; }
        protected int ParentAreaTypeId { get; private set; }
        protected string SexIds { get; private set; }
        protected string AgeIds { get; private set; }
        protected string IndicatorIds { get; private set; }
        protected string AreasCode { get; private set; }
        protected string CategoryAreaCode { get; private set; }
        protected int? ProfileId { get; private set; }
        protected string ParentAreaCode { get; private set; }
        protected int? GroupId { get; private set; }
        protected string Inequalities { get; private set; }

        /// <summary>
        /// DataInternalServicesManager constructor
        /// </summary>
        public RequestedParameters(int childAreaTypeId, int parentAreaTypeId, string parentAreaCode, string sexIds, string ageIds, string indicatorIds = null, string areasCode = null, string categoryAreaCode = null, int? profileId = null,
            int? groupId = null, string inequalities = null)
        {
            IndicatorIds = indicatorIds;
            ChildAreaTypeId = childAreaTypeId;
            ParentAreaTypeId = parentAreaTypeId;
            AreasCode = areasCode;
            CategoryAreaCode = categoryAreaCode;
            ProfileId = profileId;
            ParentAreaCode = parentAreaCode;
            GroupId = groupId;
            Inequalities = inequalities;
            SexIds = sexIds;
            AgeIds = ageIds;
        }

        /// <summary>
        /// Check null availability
        /// </summary>
        /// <returns>Exception when null</returns>
        public Exception ValidateChildAreaTypeId()
        {
            var exception = new ArgumentException("ChildAreaTypeId cannot be null");
            return ValidatorHelper.IntValid(ChildAreaTypeId) ? null : exception;
        }

        /// <summary>
        /// Check null availability
        /// </summary>
        /// <returns>Exception when null</returns>
        public Exception ValidateParentAreaTypeId()
        {
            var exception = new ArgumentException("ParentAreaTypeId cannot be null");
            return ValidatorHelper.IntValid(ParentAreaTypeId) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateIndicatorIds()
        {
            var exception = new ArgumentException("IndicatorIds cannot be null or empty");
            return ValidatorHelper.StringValid(IndicatorIds) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateAreaCode()
        {
            var exception = new ArgumentException("AreaCode cannot be null or empty");
            return ValidatorHelper.StringValid(AreasCode) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateCategoryAreaCode()
        {
            var exception = new ArgumentException("CategoryAreaCode cannot be null or empty");
            return ValidatorHelper.StringValid(CategoryAreaCode) ? null : exception;
        }

        /// <summary>
        /// Check null availability
        /// </summary>
        /// <returns>Exception when null</returns>
        public Exception ValidateProfileId()
        {
            var exception = new ArgumentException("ProfileId cannot be null");
            return ValidatorHelper.IntValid(ProfileId) ? null : exception;
        }

        /// <summary>
        /// Check null availability
        /// </summary>
        /// <returns>Exception when null</returns>
        public Exception ValidateGroupId()
        {
            var exception = new ArgumentException("GroupId cannot be null");
            return ValidatorHelper.IntValid(GroupId) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateInequalities()
        {
            var exception = new ArgumentException("Inequalities cannot be null or empty");
            return ValidatorHelper.StringValid(Inequalities) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateParentAreaCode()
        {
            var exception = new ArgumentException("ParentAreaCode cannot be null or empty");
            return ValidatorHelper.StringValid(ParentAreaCode) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateSex()
        {
            var exception = new ArgumentException("Sex cannot be null");
            return ValidatorHelper.StringValid(SexIds) ? null : exception;
        }

        /// <summary>
        /// Check null or empty availability
        /// </summary>
        /// <returns>Exception when null or empty</returns>
        public Exception ValidateAge()
        {
            var exception = new ArgumentException("Age cannot be null");
            return ValidatorHelper.StringValid(AgeIds) ? null : exception;
        }

        /// <summary>
        /// Method override into the father class
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to parse string into string array
        /// </summary>
        /// <returns></returns>
        public string[] GetCategoryCodeArray()
        {
            return ServiceHelper.StringArrayStringParser(CategoryAreaCode);
        }

        public IList<int> GetListIndicatorIds()
        {
            return ServiceHelper.IntListStringParser(IndicatorIds);
        }

        public int? GetProfileId()
        {
            return ProfileId;
        }

        public int GetChildAreaTypeId()
        {
            return ChildAreaTypeId;
        }

        public int GetParentAreaTypeId()
        {
            return ParentAreaTypeId;
        }

        public string GetParentAreaCode()
        {
            return ParentAreaCode;
        }

        public int? GetGroupId()
        {
            return GroupId;
        }

        public string GetAreasCode()
        {
            return AreasCode;
        }

        public string GetInequalities()
        {
            return Inequalities;
        }

        public IList<int> GetSexIds()
        {
            return ServiceHelper.IntListStringParser(SexIds);
        }

        public IList<int> GetAgeIds()
        {
            return ServiceHelper.IntListStringParser(AgeIds);
        }
    }
}