
using System.ComponentModel.DataAnnotations;

namespace finalproject.Models;

public class Place
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "الأسم مطلوب!")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "الاسم يجب ان يكون بين 3-30 حرف!")]
    public string Name { get; set; }

    public string Type { get; set; }

    [Required(ErrorMessage = "العنوان مطلوب!")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "العنوان يجب ان يكون بين 5-100 حرف!")]
    public string Address { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [Required(ErrorMessage = "المدينة مطلوبة!")]
    public string City { get; set; }
    
    public PlaceDetail? Detail { get; set; }


}
