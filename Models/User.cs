using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Activities.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId {get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string Email{get;set;}
        public string Password{get;set;}
        public List<Attending> Attending {get;set;}
        public User()
        {
            Attending = new List<Attending>();
        }
    }
}