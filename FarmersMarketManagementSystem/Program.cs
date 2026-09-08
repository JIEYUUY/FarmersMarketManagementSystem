using Microsoft.Extensions.DependencyInjection;
using FarmersMarketManagementSystem.Services;
using FarmersMarketManagementSystem.UI;
using FarmersMarketManagementSystem.Data;

namespace FarmersMarketManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseConnection db = new DatabaseConnection();
            db.TestConnection();

            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();

            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<DatabaseConnection>();

            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<CustomerMenu>();

            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ProductMenu>();

            services.AddSingleton<IVendorService, VendorService>();
            services.AddSingleton<VendorMenu>();

            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<OrderMenu>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            CustomerMenu customerMenu =
                serviceProvider.GetRequiredService<CustomerMenu>();

            ProductMenu productMenu =
                serviceProvider.GetRequiredService<ProductMenu>();

            VendorMenu vendorMenu =
                serviceProvider.GetRequiredService<VendorMenu>();

            OrderMenu orderMenu =
                serviceProvider.GetRequiredService<OrderMenu>();

            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                Console.WriteLine("================================");
                Console.WriteLine(" Farmers Market Management System");
                Console.WriteLine("================================");
                Console.WriteLine();
                Console.WriteLine("1. 客戶管理");
                Console.WriteLine("2. 商品管理");
                Console.WriteLine("3. 攤商管理");
                Console.WriteLine("4. 訂單管理");
                Console.WriteLine("5. 系統資訊");
                Console.WriteLine("0. 離開系統");
                Console.WriteLine();

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        customerMenu.ShowCustomerMenu();
                        break;

                    case "2":
                        productMenu.ShowProductMenu();
                        break;

                    case "3":
                        vendorMenu.ShowVendorMenu();
                        break;

                    case "4":
                        orderMenu.ShowOrderMenu();
                        break;

                    case "5":
                        Console.WriteLine();
                        Console.WriteLine("已進入系統資訊功能。");
                        Console.WriteLine("系統名稱：Farmers Market Management System。");
                        Console.WriteLine("版本：1.0。");
                        Console.WriteLine("開發者：Jieyu Ke。");
                        break;

                    case "0":
                        isRunning = false;
                        Console.WriteLine();
                        Console.WriteLine("系統已關閉。");
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～4。");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("按任意鍵返回主選單...");
                    Console.ReadKey();
                }
            }
        }
    }
}


