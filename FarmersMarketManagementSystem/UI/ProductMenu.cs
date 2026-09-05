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
        public void ShowProductMenu()
        {
            bool isProductMenuRunning = true;
            while (isProductMenuRunning)
            {
                Console.Clear();
                Console.WriteLine("""
                                    ====================
                                     商品管理
                                    ====================

                                    1. 顯示所有商品
                                    2. 搜尋商品
                                    3. 新增商品
                                    4. 修改商品
                                    0. 返回主選單

                                    """);

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllProducts();
                        break;

                    case "2":
                        SearchProduct();
                        break;

                    case "3":
                        AddProduct();
                        break;

                    case "4":
                        UpdateProduct();
                        break;

                    case "0":
                        isProductMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine("無效的選擇，請重新輸入。");
                        break;

                }
                if (isProductMenuRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("按任意鍵繼續...");
                    Console.ReadKey();
                }
            }
        }

        public void ShowProduct(Product product)
        {
            Console.WriteLine($"ID：{product.Id}");
            Console.WriteLine($"攤商 ID：{product.VendorId}");
            Console.WriteLine($"名稱：{product.ProductName}");
            Console.WriteLine($"類別：{product.Category}");
            Console.WriteLine($"庫存：{product.Quantity}");
            Console.WriteLine($"單價：{product.UnitPrice}");
        }
        public void ShowAllProducts()
        {
            List<Product> products = productService.GetAllProducts();

            foreach (Product product in products)
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

            Console.Write("請輸入廠商id：");
            int? vendorId = InputHelper.GetIntInput("");
            if (!vendorId.HasValue)
            {
                Console.WriteLine("請輸入有效的廠商id！");
                return;
            }
            product.VendorId = vendorId.Value;

            Console.Write("請輸入商品類別：");
            product.Category = Console.ReadLine() ?? "";

            Console.Write("請輸入商品名稱：");
            product.ProductName = Console.ReadLine() ?? "";

            Console.Write("請輸入商品庫存：");
            int? quantity = InputHelper.GetIntInput("");
            if (!quantity.HasValue)
            {
                Console.WriteLine("請輸入有效的商品庫存！");
                return;
            }
            product.Quantity = quantity.Value;

            decimal? unitPrice = InputHelper.GetDecimalInput("請輸入商品價格：");

            if (!unitPrice.HasValue)
            {
                Console.Write("請輸入有效的商品價格！");
                return;
            }

            product.UnitPrice = unitPrice.Value;

            productService.AddProduct(product, out string message);
            Console.WriteLine(message);
        }
        public void UpdateProduct()
        {
            int? productId = InputHelper.GetIntInput("請輸入要更新的產品ID：");

            if (productId.HasValue)
            {
                Product? product = productService.FindProductById(productId.Value);

                if (product != null)
                {
                    Console.Write("請輸入新的廠商id（留空表示不修改）：");
                    int? vendorId= InputHelper.GetIntInput("");

                    Console.WriteLine("請輸入新的商品類別（留空表示不修改）：");
                    string? category = Console.ReadLine();

                    Console.WriteLine("請輸入新的商品名稱（留空表示不修改）：");
                    string? name = Console.ReadLine();

                    Console.Write("請輸入新的商品庫存（留空表示不修改）：");
                    int? quantity = InputHelper.GetIntInput("");

                    decimal? unitPrice = InputHelper.GetDecimalInput("請輸入新的商品價格（留空表示不修改）：");

                    bool isUpdated = productService.UpdateProductInformation(product,vendorId,category,name,quantity,unitPrice);

                    if (isUpdated)
                    {
                        Console.WriteLine("商品更新成功！");
                    }
                    else
                    {
                        Console.WriteLine("商品更新失敗。");
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