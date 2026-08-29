using LegendaryCruises.Models;
using System.ComponentModel.DataAnnotations;

public class Cruise
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Destination { get; set; } = string.Empty;

    public string? DeparturePort { get; set; }
    public string? ArrivalPort { get; set; }

    

    public string Currency { get; set; } = "EUR";

    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }

    public int MaxPassengers { get; set; }

    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public string Slug { get; set; } = "";

    public string? CreatedByUserId { get; set; }


    public ICollection<CruiseDate> CruiseDates { get; set; } = new List<CruiseDate>();

    public ICollection<ItineraryDay> Itinerary { get; set; } = new List<ItineraryDay>();
}