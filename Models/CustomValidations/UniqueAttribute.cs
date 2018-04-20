using System.ComponentModel.DataAnnotations;
using System.Linq;
using Activities.Models;

namespace Activities.Models
{
    public class UniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (ActivitiesContext) validationContext.GetService(typeof(ActivitiesContext));
            var allUsers = _context.users;
            foreach(var each in allUsers)
            {
                if((string)value == (string)each.Email)
                {
                    return new ValidationResult("Email already exists in database");
                }
            }
            return ValidationResult.Success;
        }
    }
}