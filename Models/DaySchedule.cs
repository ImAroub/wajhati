namespace finalproject.Models;

public class DaySchedule
{
    public int Id { get; set; }
    
    public int DayNumber { get; set; }

    public int TripPlanId { get; set; }

    public TripPlan TripPlan { get; set; }

    public List<DaySchedulePlace> DaySchedulePlaces { get; set; }

}
