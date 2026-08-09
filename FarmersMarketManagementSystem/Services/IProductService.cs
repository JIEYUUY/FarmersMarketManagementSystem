using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IProductService
    {
        Product? FindProductById(int id);
        bool AddProduct(Product product);

        List<Product> GetAllProducts();

        void UpdateProductInformation(Product product, string? newName, string? newCategory, decimal? newPrice);

        bool DeleteProduct(Product product);
    }
}
