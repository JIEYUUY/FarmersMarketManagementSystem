namespace FarmersMarketManagementSystem.Models
{
    internal class OrderDetail
    {
        public int OrderId;

        public string CustomerName = "";

        public DateTime OrderDate;

        public decimal TotalPrice;

        public List<OrderDetailItem> Items = new List<OrderDetailItem>();
    }
}