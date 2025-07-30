using Microsoft.EntityFrameworkCore;
using finalproject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace finalproject.Data
{

    public class AppDbContext : IdentityDbContext<IdentityUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Place> places { get; set; }

        public DbSet<PlaceDetail> placesDetails { get; set; }

        public DbSet<TripPlan> tripPlans { get; set; }

        public DbSet<DaySchedulePlace> daySchedulePlaces { get; set; }

        public DbSet<DaySchedule> daySchedules { get; set; }
        
       
    }
}