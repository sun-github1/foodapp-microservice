using Food.Services.OrderAPI.Models;

namespace Food.Services.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<OrderHeader> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}
