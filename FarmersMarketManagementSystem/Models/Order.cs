namespace FarmersMarketManagementSystem.Models
{
    internal class Order
    {
        public int Id;

        public int CustomerId;

        public DateTime OrderDate;

        public OrderStatus Status;

        public decimal SubTotal;

        public List<OrderItem> Items = new List<OrderItem>();

        public decimal CalculateSubTotal()
        {
            decimal SubTotal = 0;

            foreach (OrderItem item in Items)
            {
                SubTotal += item.Quantity * item.UnitPrice;
            }

            return SubTotal;
        }
        public decimal ShippingFee;
        public decimal TotalPrice;
        public int? MergedIntoOrderId;
    }
}
