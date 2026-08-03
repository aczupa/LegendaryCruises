namespace LegendaryCruises.Models.DTOs
{
    public class CruiseDateDto
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public List<DateCabinDto> Cabins { get; set; } = new();
    }
}
