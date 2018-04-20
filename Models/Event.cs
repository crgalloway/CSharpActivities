using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Activities.Models
{
    public class Event : BaseEntity
    {
        [Key]
        public int EventId {get;set;}
        public string Title{get;set;}
        public string Description{get;set;}
        public DateTime StartTime{get;set;}
        public int DurationInMinutes{get;set;}

        [ForeignKey("User")]
        public int CreatedByUserId{get;set;}
        public User CreatedBy{get;set;}
        public List<Attending> Attendees {get;set;}
        public Event()
        {
            Attendees = new List<Attending>();
        }
    }
}