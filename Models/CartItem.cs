namespace LegendaryCruises.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int CruiseId { get; set; }
        public Cruise Cruise { get; set; } = null!;

        public int CruiseDateId { get; set; }
        public CruiseDate CruiseDate { get; set; } = null!;

        public int DateCabinId { get; set; }
        public DateCabin DateCabin { get; set; } = null!;

        public CabinType CabinType { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total => Price * Quantity;
    }
}
