
    namespace LegendaryCruises.Models
    {
        public class Order
        {
            public int Id { get; set; }

            public string UserId { get; set; } = string.Empty;

            public decimal TotalPrice { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public bool IsPaid { get; set; } = false;

            // Powiązanie z OrderItem
            public List<OrderItem> Items { get; set; } = new();
        }
    

}
