using LegendaryCruises.Models;

public class CruiseDate
{
    public int Id { get; set; }

    public int CruiseId { get; set; }
    public Cruise? Cruise { get; set; }

    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }

    public int DurationDays =>
     Math.Max(1, (ReturnDate - DepartureDate).Days + 1);

    public ICollection<DateCabin> Cabins { get; set; } = new List<DateCabin>();
}