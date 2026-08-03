using LegendaryCruises.Models;

namespace LegendaryCruises.Interfaces
{
    public interface IPassengerService
    {
        Task SavePassenger(string userId, PassengerInfo passenger);
    }
}
