using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Activities.Models
{
    public class UserViewModel : BaseEntity
    {
        [Display(Name = "First Name: ")]
        [Required(ErrorMessage="First name must not be blank")]
        [MinLength(2, ErrorMessage="First name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage="First name cannot contain numerals")]
        public string FirstName{get;set;}
        [Display(Name = "Last Name: ")]
        [Required(ErrorMessage="Last name must not be blank")]
        [MinLength(2, ErrorMessage="Last name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage="Last name cannot contain numerals")]
        public string LastName{get;set;}
        [EmailAddress]
        [Display(Name="Email: ")]
        [Required(ErrorMessage="Email must not be blank")]
        [Unique]
        public string Email{get;set;}
        [MinLength(8)]
        [Display(Name="Password: ")]
        [Required(ErrorMessage="Password must not be blank")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$", ErrorMessage="Password must contain at least 1 number, 1 letter, and a special character")]
        public string Password{get;set;}
        [Compare("Password", ErrorMessage="Password Confirmation must match")]
        [Display(Name="Confirm Password: ")]
        [Required(ErrorMessage="Password Confirmation must not be blank")]
        public string PasswordConfirmation{get;set;}
    }
}