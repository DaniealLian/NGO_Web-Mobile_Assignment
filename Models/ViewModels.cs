using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NGO_Web_Demo.Models;

// View Models ----------------------------------------------------------------

#nullable disable warnings

public class EventInsertVM
{
    [StringLength(4)]
    [RegularExpression(@"E\d{3}", ErrorMessage = "Invalid {0} format.")] // FIXED: Changed from P to E
    [Remote("CheckId", "NGO_Event_", ErrorMessage = "Duplicated {0}.")]
    [Display(Name = "Event ID")]
    public string Event_Id { get; set; }

    [Required(ErrorMessage = "Event title is required")]
    [StringLength(100, ErrorMessage = "Event title cannot exceed 100 characters")]
    [Display(Name = "Event Title")]
    public string Event_Title { get; set; }

    [Required(ErrorMessage = "Event date is required")]
    [Display(Name = "Event Date")]
    [DataType(DataType.Date)]
    public DateTime Event_Date { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Event Description")]
    public string Event_Description { get; set; }

    [Required(ErrorMessage = "Event location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Event Location")]
    public string Event_Location { get; set; }

    // Other properties
    [Display(Name = "Event Photo")]
    public IFormFile Event_Photo { get; set; }
}

public class EventUpdateVM
{
    [Display(Name = "Event ID")]
    public string Event_Id { get; set; }

    [Required(ErrorMessage = "Event title is required")]
    [StringLength(100, ErrorMessage = "Event title cannot exceed 100 characters")]
    [Display(Name = "Event Title")]
    public string Event_Title { get; set; }

    [Required(ErrorMessage = "Event date is required")]
    [Display(Name = "Event Date")]
    [DataType(DataType.Date)]
    public DateTime Event_Date { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Event Description")]
    public string Event_Description { get; set; }

    [Required(ErrorMessage = "Event location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Event Location")]
    public string Event_Location { get; set; }

    // Other properties
    [Display(Name = "Current Photo")]
    public string? Event_PhotoURL { get; set; }

    [Display(Name = "New Photo")]
    public IFormFile? Event_Photo { get; set; }
}