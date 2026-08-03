namespace LegendaryCruises.Models.Responses
{
    public class GetCruisesResponse : BaseResponse
    {
        public List<Cruise> Cruises { get; set; } = new();
    }
}
