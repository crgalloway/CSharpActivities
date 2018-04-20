using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Activities.Models
{
    public class EventViewModel : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string Title{get;set;}
        [Required]
        [MinLength(10)]
        public string Description{get;set;}
        [Required]
        [InTheFuture]
        public DateTime? StartDate{get;set;}
        [Required]
        public DateTime? StartTime{get;set;}
        [Range(1,Int32.MaxValue)]
        public int Duration{get;set;}
        public string DurationUnits{get;set;}
        public int CreatedByUserId{get;set;}
    }
}