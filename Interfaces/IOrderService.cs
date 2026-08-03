using LegendaryCruises.Models;

namespace LegendaryCruises.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(string userId);
        Task MarkAsPaid(int orderId);
        Task<Order?> GetOrderById(int orderId);
        Task ProcessOrderAfterPayment(int orderId);
    }
}
