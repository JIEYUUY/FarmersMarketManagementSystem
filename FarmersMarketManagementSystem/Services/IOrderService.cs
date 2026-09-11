using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IOrderService
    {
        bool AddItemToOrder(
            Order order,
            int productId,
            int quantity,
            out string message);
        bool CreateOrder(Order order, out string message);
        Order? GetOrderById(int id);
        OrderDetail? GetOrderDetail(int orderId);
    }
}