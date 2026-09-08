using FarmersMarketManagementSystem.Models;
using FarmersMarketManagementSystem.Services;
using FarmersMarketManagementSystem.Utilities;

namespace FarmersMarketManagementSystem.UI
{
    internal class OrderMenu
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ICustomerService customerService;

        public OrderMenu(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.customerService = customerService;
        }

        public void ShowOrderMenu()
        {
            bool isOrderMenuRunning = true;

            while (isOrderMenuRunning)
            {
                Console.Clear();

                Console.WriteLine("""
                    ====================
                     訂單管理
                    ====================

                    1. 建立訂單
                    0. 返回主選單

                    """);

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateOrder();
                        break;

                    case "0":
                        isOrderMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine("輸入錯誤，請輸入 0～1。");
                        break;
                }

                if (isOrderMenuRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("按任意鍵繼續...");
                    Console.ReadKey();
                }
            }
        }


        private void ShowProducts()
        {
            Console.WriteLine("目前商品：");

            List<Product> products =
                productService.GetAllProducts();

            foreach (Product product in products)
            {
                Console.WriteLine(
                    $"ID：{product.Id} | " +
                    $"{product.ProductName} | " +
                    $"庫存：{product.Quantity} | " +
                    $"單價：{product.UnitPrice}");
            }
        }

        private void ShowCurrentOrder(Order order)
        {
            if (order.Items.Count == 0)
            {
                Console.WriteLine("目前訂單沒有商品。");
                return;
            }

            foreach (OrderItem item in order.Items)
            {
                Console.WriteLine(
                    $"ProductId：{item.ProductId} | " +
                    $"數量：{item.Quantity} | " +
                    $"單價：{item.UnitPrice} | " +
                    $"小計：{item.Quantity * item.UnitPrice}");
            }
        }
        private void CreateOrder()
        {
            Order order = new Order();

            int? customerId =
                InputHelper.GetIntInput("請輸入客戶 ID：");

            if (!customerId.HasValue)
            {
                Console.WriteLine("請輸入有效的客戶 ID。");
                return;
            }

            Customer? customer =
                customerService.GetCustomer(customerId.Value);

            if (customer == null)
            {
                Console.WriteLine("找不到此客戶。");
                return;
            }

            order.CustomerId = customerId.Value;
            order.OrderDate = DateTime.Now;

            bool isAddingItems = true;

            while (isAddingItems)
            {
                Console.WriteLine();
                ShowProducts();

                int? productId =
                    InputHelper.GetIntInput("請輸入商品 ID：");

                if (!productId.HasValue)
                {
                    Console.WriteLine("請輸入有效的商品 ID。");
                    continue;
                }

                int? quantity =
                    InputHelper.GetIntInput("請輸入購買數量：");

                if (!quantity.HasValue)
                {
                    Console.WriteLine("請輸入有效的購買數量。");
                    continue;
                }

                orderService.AddItemToOrder(
                    order,
                    productId.Value,
                    quantity.Value,
                    out string itemMessage);

                Console.WriteLine(itemMessage);

                Console.Write("是否繼續加入商品？(Y/N)：");
                string? confirm = Console.ReadLine();

                if (confirm?.Trim().ToUpper() != "Y")
                {
                    isAddingItems = false;
                }
            }

            if (order.Items.Count == 0)
            {
                Console.WriteLine("訂單沒有任何商品，無法建立。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("訂單內容：");

            ShowCurrentOrder(order);

            order.TotalPrice = order.CalculateTotalPrice();

            Console.WriteLine($"訂單總金額：{order.TotalPrice}");

            Console.Write("確定建立訂單嗎？(Y/N)：");
            string? createConfirm = Console.ReadLine();

            if (createConfirm?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("已取消建立訂單。");
                return;
            }

            if (orderService.CreateOrder(order, out string message))
            {
                Console.WriteLine(message);
                Console.WriteLine($"訂單編號：{order.Id}");
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}