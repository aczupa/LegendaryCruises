namespace LegendaryCruises.Models
{
    public class ItineraryDay
    {
        public int Id { get; set; }

        public int CruiseId { get; set; }

        public int DayNumber { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Cruise? Cruise { get; set; }
    }
}

