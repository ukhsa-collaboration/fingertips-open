using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.UserAccess.DataValidation.CustomValidation
{
    public class DuplicateNameCheck : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isExist = false;
            IndicatorList indicatorList = validationContext.ObjectInstance as IndicatorList;
            if (indicatorList != null)
            {
                using (FINGERTIPS_USERSEntities dbContext = new FINGERTIPS_USERSEntities())
                {
                    isExist = dbContext.IndicatorLists
                        .Any(d => d.ListName.ToLower() == indicatorList.ListName.ToLower()
                                  && d.UserId == indicatorList.UserId
                                  && d.Id != indicatorList.Id);
                }
            }
            if (isExist)
            {
                return new ValidationResult
                    ("The List Name already exist", new string[] {"ListName"});
            }
            return ValidationResult.Success;
        }
       
   
    }
}
