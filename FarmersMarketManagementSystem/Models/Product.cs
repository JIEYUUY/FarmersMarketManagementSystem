namespace FarmersMarketManagementSystem.Models
{
    internal class Product
    {
        public int Id;

        public int VendorId;

        public string Category = "";

        public string ProductName = "";

        public int Quantity;

        public decimal UnitPrice;
    }
}