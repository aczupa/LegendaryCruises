using LegendaryCruises.Models;
using LegendaryCruises.Pages;

namespace LegendaryCruises.Interfaces
{
    public interface IUserProfileService
    {
        Task SaveUserProfile(string userId, CheckoutInfo info);
    }
}
