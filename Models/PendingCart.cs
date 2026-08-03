namespace LegendaryCruises.Models
{
    public class PendingCart
    {
        public string Slug { get; set; } = "";
        public int CruiseId { get; set; }
        public int CruiseDateId { get; set; }

        public Dictionary<int, int> Cabins { get; set; } = new();
    }
}
