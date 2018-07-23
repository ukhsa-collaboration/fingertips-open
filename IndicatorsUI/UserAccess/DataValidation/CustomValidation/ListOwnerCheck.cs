using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsUI.UserAccess.DataValidation.CustomValidation
{
    public class ListOwnerCheck : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isOwner = false;
            IndicatorList indicatorList = validationContext.ObjectInstance as IndicatorList;

            if (indicatorList != null)
            {
                if (indicatorList.Id == 0)
                {
                    isOwner = true;
                }
                else
                {
                   using (FINGERTIPS_USERSEntities dbContext = new FINGERTIPS_USERSEntities())
                    {
                        var list = dbContext.IndicatorLists.Where(d => d.UserId == indicatorList.UserId);
                        if (list != null)
                        {
                            foreach (var ind in list)
                            {
                                if (ind.Id == indicatorList.Id)
                                    isOwner = true;
                            }
                        }

                    }
                }
            }
            if (!isOwner)
            {
                return new ValidationResult
                    ("Can't edit list as user is not the owner of the list", new string[] { "ListName" });
            }
            return ValidationResult.Success;
        }
    }
}
