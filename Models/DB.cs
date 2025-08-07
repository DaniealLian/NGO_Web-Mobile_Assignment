using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models;

#nullable disable warnings

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    // DB Sets
    public DbSet<Event> Events { get; set; }
}

// Entity Classes

public class Event
{
    [Key, MaxLength(4)]
    public string EventID { get; set; }
    [MaxLength(100)]
    public string EventTitle { get; set; }
    [MaxLength(200)]
    public DateTime EventDate { get; set; }
    [MaxLength(100)]
    public string EventLocation { get; set; }
    [MaxLength(500)]
    public string EventDescription { get; set; }

    public string? EventPhotoURL { get; set; }
}
