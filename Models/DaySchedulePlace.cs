namespace finalproject.Models;

public class DaySchedulePlace
{
    public int Id { get; set; }

    public int DayScheduleId { get; set; }
    public DaySchedule DaySchedule { get; set; }

    public int PlaceId { get; set; }
    public Place Place { get; set; }

    public bool IsMainPlace { get; set; }
    public int OrderInDay { get; set; }
}
