using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class ProductService : IProductService
    {
        private List<Product> products = new List<Product>();
        private int nextProductId = 1;
    }
}
