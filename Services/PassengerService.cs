using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;

namespace LegendaryCruises.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly DataContext _context;

        public PassengerService(DataContext context)
        {
            _context = context;
        }

        public async Task SavePassenger(string userId, PassengerInfo passenger)
        {
            passenger.UserId = userId;

            _context.PassengerInfos.Add(passenger);
            await _context.SaveChangesAsync();
        }
    }
}
