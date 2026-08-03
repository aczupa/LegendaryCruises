using LegendaryCruises.Models;
using LegendaryCruises.Models.Responses;

namespace LegendaryCruises.Interfaces
{
    public interface ICartService
    {
        Task<GetCartItemResponse> AddToCart(string userId, int cruiseId, int cruiseDateId, int dateCabinId, int quantity);
        Task<Cart> GetCart(string userId);
        Task<int> GetCartItemCount(string userId);
        Task<GetCartItemResponse> RemoveFromCart(string userId, int cartItemId);
        Task<GetCartItemResponse> UpdateQuantity(string userId, int cartItemId, int newQty);
     
        Task ClearCart(string userId);
    }

}
