namespace LegendaryCruises.Models
{
    public class DateCabin
    {
        public int Id { get; set; }

        public int CruiseDateId { get; set; }
        public CruiseDate? CruiseDate { get; set; }

        public CabinType CabinType { get; set; }

        public decimal Price { get; set; }

        public int Capacity { get; set; }
        
        public int Reserved { get; set; }
        public int Available => Capacity - Reserved;
    }
}
