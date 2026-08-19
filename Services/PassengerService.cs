using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IDbContextFactory<DataContext> _factory;

        public PassengerService(IDbContextFactory<DataContext> factory)
        {
            _factory = factory;
        }

        public async Task SavePassenger(string userId, PassengerInfo passenger)
        {
            await using var context = _factory.CreateDbContext();

            passenger.UserId = userId;

            context.PassengerInfos.Add(passenger);
            await context.SaveChangesAsync();
        }
    }
}
