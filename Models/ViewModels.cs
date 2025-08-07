using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NGO_Web_Demo.Models;

// View Models ----------------------------------------------------------------

#nullable disable warnings

public class EventInsertVM
{
    [StringLength(4)]
    [RegularExpression(@"P\d{3}", ErrorMessage = "Invalid {0} format.")]
    [Remote("CheckId", "Product", ErrorMessage = "Duplicated {0}.")]
    public string Event_Id { get; set; }

    [StringLength(100)]
    public string Event_Title { get; set; }

    //[Range(0.01, 9999.99)]
    //[RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid {0} format.")]
    //public decimal Price { get; set; }

    public DateTime Event_Date { get; set; }
    
    [StringLength(500)]
    public string Event_Description { get; set; }

    public string Event_Location { get; set; }

    // Other properties
    public IFormFile Event_Photo { get; set; }
}

public class EventUpdateVM
{
    public string Event_Id { get; set; }

    [StringLength(100)]
    public string Event_Title { get; set; }

    //[Range(0.01, 9999.99)]
    //[RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid {0} format.")]
    //public decimal Price { get; set; }

    public DateTime Event_Date { get; set; }

    [StringLength(500)]
    public string Event_Description { get; set; }

    public string Event_Location { get; set; }

    // Other properties
    public string? Event_PhotoURL { get; set; }
    public IFormFile? Event_Photo { get; set; }
}
