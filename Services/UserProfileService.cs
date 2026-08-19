using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IDbContextFactory<DataContext> _factory;

        public UserProfileService(IDbContextFactory<DataContext> factory)
        {
            _factory = factory;
        }

        public async Task SaveUserProfile(string userId, CheckoutInfo info)
        {
            await using var context = _factory.CreateDbContext();

            var userProfile = new UserProfile
            {
                UserId = userId,
                FirstName = info.FirstName,
                LastName = info.LastName,
                Street = info.Street,
                StreetNumber = info.StreetNumber,
                PostalCode = info.PostalCode,
                City = info.City,
                Country = info.Country,
                PaymentMethod = info.PaymentMethod,
                Phone = info.Phone,
                PassportNumber = info.PassportNumber
            };

            context.UserProfiles.Add(userProfile);
            await context.SaveChangesAsync();
        }
    }
}
