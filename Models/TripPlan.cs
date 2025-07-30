using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace finalproject.Models;

public class TripPlan
{

    [Key]
    public int Id { get; set; }

    public string City { get; set; }

    public int NumberOfDays { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }

    public List<DaySchedule> DailySchedules { get; set; }

}
