using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class ProductService : IProductService
    {
        private List<Product> products = new List<Product>();
        private int nextProductId = 1;
        public Product? FindProductById(int id)
        {
            foreach (Product product in products)
            {
                if (product.Id == id)
                {
                    return product;
                }
            }
            return null;
        }
        public bool AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return false;
            }

            product.Id = nextProductId;
            nextProductId++;

            products.Add(product);

            return true;
        }
        public List<Product> GetAllProducts()
        {
            return products;
        }
        public void UpdateProductInformation(Product product, string? newName, string? newCategory, decimal? newPrice)
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                product.Name = newName;
            }
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                product.Category = newCategory;
            }
            if (newPrice.HasValue)
            {
                product.Price = newPrice.Value;
            }
        }
        public bool DeleteProduct(Product product)
        {
            return products.Remove(product);
        }
    }
}
