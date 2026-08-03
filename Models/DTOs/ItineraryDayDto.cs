using System.ComponentModel.DataAnnotations;

namespace LegendaryCruises.Models.DTOs
{
    public class ItineraryDayDto
    {
        [Range(1, 365)]
        public int DayNumber { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}