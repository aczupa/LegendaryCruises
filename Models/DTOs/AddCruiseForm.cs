using LegendaryCruises.Models.DTOs;
using System.ComponentModel.DataAnnotations;

public class AddCruiseForm
{
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

    [Range(1, 10000)]
    public int MaxPassengers { get; set; }

    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;


    public List<CruiseDateDto> Dates { get; set; } = new();

    public List<ItineraryDayDto> Itinerary { get; set; } = new();
}