using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IProductService
    {
        Product? FindProductById(int id);

        bool AddProduct(Product product, out string message);

        List<Product> GetAllProducts();

        bool UpdateProductInformation(Product product, int? newVendorId, string? newCategory, string? newProductName, int? newQuantity, decimal? newUnitPrice);

    }
}
