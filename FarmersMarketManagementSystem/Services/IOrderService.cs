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
        List<Order> GetOrdersByCustomer(int customerId);
        bool UpdateOrderStatus(int orderId,OrderStatus newStatus,out string message);
        bool CancelOrder(int orderId,out string message);
        bool MergeOrders(int orderId1, int orderId2, out string message);
        decimal CalculateShippingFee(decimal orderSubTotal);
    }
}