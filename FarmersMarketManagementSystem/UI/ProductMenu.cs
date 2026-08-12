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
        public void UpdateProduct()
        {
            int? productId = InputHelper.GetIntInput("請輸入要更新的產品ID：");

            if (productId.HasValue)
            {
                Product? product = productService.FindProductById(productId.Value);

                if (product != null)
                {
                    Console.WriteLine("請輸入新的商品名稱（留空表示不修改）：");
                    string? name = Console.ReadLine();
                    
                    Console.WriteLine("請輸入新的商品類別（留空表示不修改）：");
                    string? category = Console.ReadLine();
                    
                    decimal? price = InputHelper.GetDecimalInput("請輸入新的商品價格（留空表示不修改）：");

                    productService.UpdateProductInformation(product, name, category, price);

                    Console.WriteLine("商品更新成功！");
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
        public void DeleteProduct()
        {
            int? productId = InputHelper.GetIntInput("請輸入要刪除的產品ID：");
            if (productId.HasValue)
            {
                Product? product = productService.FindProductById(productId.Value);
                if (product != null)
                {
                    ShowProduct(product);
                    Console.WriteLine("確定要刪除嗎？(Y/N)");

                    string? confirm = Console.ReadLine();
                    if(confirm?.ToUpper() == "Y")
                    {
                        if (productService.DeleteProduct(product))
                        {
                            Console.WriteLine("商品刪除成功！");
                        }
                        else
                        {
                            Console.WriteLine("商品刪除失敗！");
                        }
                    }
                    else
                    {
                        Console.WriteLine("已取消刪除。");
                    }
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
    }
}