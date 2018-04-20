using System.ComponentModel.DataAnnotations;
using System;

namespace Activities.Models
{
    public class InTheFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime now = DateTime.Now;
            if(value != null)
            {
                var timeDiff = DateTime.Compare((DateTime)value, now);
                if(timeDiff<0)
                {
                    return new ValidationResult("Date of event must be in the future");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Date of event must be in the future");
        }
    }
}