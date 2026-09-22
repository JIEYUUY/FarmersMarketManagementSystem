namespace FarmersMarketManagementSystem.Models
{
    internal class OrderDetail
    {
        public int OrderId;

        public string CustomerName = "";

        public DateTime OrderDate;

        public decimal TotalPrice;
        public decimal SubTotal;
        public decimal ShippingFee;

        public List<OrderDetailItem> Items = new List<OrderDetailItem>();

        public OrderStatus Status;

        public int? MergedIntoOrderId;
    }
}