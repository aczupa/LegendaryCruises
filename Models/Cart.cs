namespace LegendaryCruises.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public List<CartItem> Items { get; set; } = new();

        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    }

}
