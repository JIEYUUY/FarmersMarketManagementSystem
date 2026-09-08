namespace FarmersMarketManagementSystem.Models
{
    internal class Order
    {
        public int Id;

        public int CustomerId;

        public DateTime OrderDate;

        public decimal TotalPrice;

        public List<OrderItem> Items = new List<OrderItem>();

        public decimal CalculateTotalPrice()
        {
            decimal total = 0;

            foreach (OrderItem item in Items)
            {
                total += item.Quantity * item.UnitPrice;
            }

            return total;
        }
    }
}
