using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class AmenityViewModel : Amenity
{
}

[ExcludeFromCodeCoverage]
public class EditableAmenityViewModel
{
    public int AmenityId { get; set; }

    [Display(Name = "Amenity Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Amenity Name is required")]
    [MaxLength(100)]
    public string AmenityName { get; set; }

    [Display(Name = "Amenity Description")]
    [MaxLength(1000)]
    public string AmenityDescription { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class AmenitiesViewModel
{
    public IEnumerable<AmenityViewModel> Amenities { get; set; }
    public bool CanEdit { get; set; }
}
