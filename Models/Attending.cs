using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Activities.Models
{
    public class Attending : BaseEntity
    {
        [Key]
        public int AttendingID{get;set;}
        [ForeignKey("User")]
        public int AttendingUserId{get;set;}
        public User User{get;set;}
        [ForeignKey("Event")]
        public int AttendingEventId{get;set;}
        public Event Event{get;set;}
    }
}