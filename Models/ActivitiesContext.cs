using Microsoft.EntityFrameworkCore;
using Activities.Models;

namespace Activities.Models
{
    public class ActivitiesContext : DbContext
    {
        public ActivitiesContext(DbContextOptions<ActivitiesContext> options) : base(options) {}
        public DbSet<User> users {get;set;}
        public DbSet<Event> events {get;set;}
        public DbSet<Attending> attending {get;set;}
    }
}