using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finalproject.Models;
public class PlaceDetail
{
    [Key]
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? OpeningHours { get; set; }

     public bool RequiresBooking { get; set; } 

    public string? Website { get; set; }

    public string? ImageUrl { get; set; }

    public int PlaceId { get; set; }

    [ForeignKey("PlaceId")]// اي حقل هو الفورنك
    public Place Place { get; set; }//ايش الكلاس المرتبط به
}
