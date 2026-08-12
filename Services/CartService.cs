using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using LegendaryCruises.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Services
{
    public class CartService : ICartService
    {
        private readonly IDbContextFactory<DataContext> _factory;

        public CartService(IDbContextFactory<DataContext> factory)
        {
            _factory = factory;
        }

        public async Task<GetCartItemResponse> AddToCart(
            string userId,
            int cruiseId,
            int cruiseDateId,
            int dateCabinId,
            int quantity)
        {
            await using var context = _factory.CreateDbContext();

            var cabin = await context.DateCabins
                .Include(c => c.CruiseDate)
                .FirstOrDefaultAsync(c => c.Id == dateCabinId);

            if (cabin == null)
                return new GetCartItemResponse { Success = false, Message = "Cabine introuvable." };

            int available = cabin.Available;

            var cart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                context.Carts.Add(cart);
                await context.SaveChangesAsync();
            }

            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.DateCabinId == dateCabinId);

            int alreadyInCart = cartItem?.Quantity ?? 0;
            int totalRequested = alreadyInCart + quantity;

            if (totalRequested > available)
            {
                return new GetCartItemResponse
                {
                    Success = false,
                    Message = $"Quantité disponible : {available - alreadyInCart}"
                };
            }

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    CruiseId = cruiseId,
                    CruiseDateId = cruiseDateId,
                    DateCabinId = dateCabinId,
                    CabinType = cabin.CabinType,
                    Price = cabin.Price,
                    Quantity = quantity
                };

                context.CartItems.Add(cartItem);
            }

            await context.SaveChangesAsync();

            return new GetCartItemResponse
            {
                Success = true,
                Message = "Cabine ajoutée au panier.",
                CartItem = cartItem
            };
        }

        public async Task<Cart> GetCart(string userId)
        {
            await using var context = _factory.CreateDbContext();

            return await context.Carts
     .AsNoTracking()
     .Include(c => c.Items)
         .ThenInclude(i => i.DateCabin)
     .Include(c => c.Items)
         .ThenInclude(i => i.Cruise)
     .Include(c => c.Items)
         .ThenInclude(i => i.CruiseDate)
     .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<int> GetCartItemCount(string userId)
        {
            await using var context = _factory.CreateDbContext();

            return await context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }

        public async Task<GetCartItemResponse> RemoveFromCart(string userId, int cartItemId)
        {
            await using var context = _factory.CreateDbContext();

            var cartItem = await context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null || cartItem.Cart.UserId != userId)
            {
                return new GetCartItemResponse { Success = false, Message = "Élément introuvable." };
            }

            context.CartItems.Remove(cartItem);
            await context.SaveChangesAsync();

            return new GetCartItemResponse { Success = true, Message = "Élément supprimé." };
        }

        public async Task<GetCartItemResponse> UpdateQuantity(string userId, int cartItemId, int newQty)
        {
            await using var context = _factory.CreateDbContext();

            var cartItem = await context.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.DateCabin)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null || cartItem.Cart.UserId != userId)
            {
                return new GetCartItemResponse { Success = false, Message = "Élément introuvable." };
            }

            int available = cartItem.DateCabin.Available;

            if (newQty > available)
            {
                return new GetCartItemResponse
                {
                    Success = false,
                    Message = $"Quantité max disponible : {available}"
                };
            }

            cartItem.Quantity = newQty;
            await context.SaveChangesAsync();

            return new GetCartItemResponse { Success = true, Message = "Quantité mise à jour." };
        }

        public async Task ClearCart(string userId)
        {
            await using var context = _factory.CreateDbContext();

            var cart = await context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                context.CartItems.RemoveRange(cart.Items);
                await context.SaveChangesAsync();
            }
        }
    }
}
