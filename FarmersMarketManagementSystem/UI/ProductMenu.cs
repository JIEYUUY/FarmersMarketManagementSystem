using FarmersMarketManagementSystem.Models;
using FarmersMarketManagementSystem.Services;
using FarmersMarketManagementSystem.Utilities;

namespace FarmersMarketManagementSystem.UI
{
    internal class ProductMenu
    {
        private readonly IProductService productService;

        public ProductMenu(IProductService productService)
        {
            this.productService = productService;
        }
        public void ShowProduct(Product product)
        {
            Console.WriteLine($"ID：{product.Id}");
            Console.WriteLine($"名稱：{product.Name}");
            Console.WriteLine($"類別：{product.Category}");
            Console.WriteLine($"價格：{product.Price}");
        }
        public void ShowAllProducts()
        {
            List<Product> products = productService.GetAllProducts();
            foreach (var product in products)
            {
                ShowProduct(product);
            }
        }

        public void SearchProduct()
        {
            int? productId = InputHelper.GetIntInput("請輸入產品ID：");

            if (productId.HasValue)
            {
                Product? product = productService.FindProductById(productId.Value);

                if (product != null)
                {
                    ShowProduct(product);
                }
                else
                {
                    Console.WriteLine("未找到該產品。");
                }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字！");
            }
        }
        public void AddProduct()
        {
            Product product = new Product();

            Console.WriteLine("請輸入商品名稱：");
            product.Name = Console.ReadLine() ?? "";

            Console.WriteLine("請輸入商品類別：");
            product.Category = Console.ReadLine() ?? "";

            decimal? price = InputHelper.GetDecimalInput("請輸入商品價格：");

            if (!price.HasValue)
            {
                Console.WriteLine("請輸入有效的商品價格！");
                return;
            }
             
            product.Price = price.Value;

            if (productService.AddProduct(product))
            {
                Console.WriteLine("添加商品成功！");
            }
            else
            {
                Console.WriteLine("添加商品失敗！");
            }
        }
    }
}