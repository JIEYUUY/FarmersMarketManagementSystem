using FarmersMarketManagementSystem.Models;
using FarmersMarketManagementSystem.Services;
using FarmersMarketManagementSystem.Utilities;

namespace FarmersMarketManagementSystem.UI
{
    internal class CustomerMenu
    {
        private readonly ICustomerService customerService;
        public CustomerMenu(ICustomerService customerService)
        {
            this.customerService = customerService;
        }
        public void ShowCustomer(Customer customer)
        {
            Console.WriteLine($"ID：{customer.Id}");
            Console.WriteLine($"姓名：{customer.Name}");
            Console.WriteLine($"電話：{customer.Phone}");
            Console.WriteLine($"城市：{customer.City}");
        }
        public void ShowAllCustomers()
        {
            Console.WriteLine();

            List<Customer> allCustomers = customerService.GetAllCustomers();

            foreach (Customer customer in allCustomers)
            {
                ShowCustomer(customer);
            }
        }
        public void ShowCustomerMenu()
        {
            bool isCustomerMenuRunning = true;

            while (isCustomerMenuRunning)
            {
                Console.Clear();

                Console.WriteLine("""
        ====================
         客戶管理
        ====================

        1. 顯示所有客戶
        2. 搜尋客戶
        3. 新增客戶
        4. 修改客戶
        5. 刪除客戶
        0. 返回主選單

        """);

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllCustomers();
                        break;

                    case "2":
                        SearchCustomer();
                        break;

                    case "3":
                        AddCustomer();
                        break;

                    case "4":
                        UpdateCustomer();
                        break;

                    case "5":
                        DeleteCustomer();
                        break;

                    case "0":
                        isCustomerMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～5。");
                        break;
                }

                if (isCustomerMenuRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("按任意鍵繼續...");
                    Console.ReadKey();
                }
            }
        }
        public void SearchCustomer()
        {
            int? searchId = InputHelper.GetIntInput("請輸入客戶 ID：");
            if (searchId != null)
            {
                Customer? customer = customerService.FindCustomerById(searchId.Value);

                if (customer != null)
                {
                    ShowCustomer(customer);
                }
                else
                {
                    Console.WriteLine("找不到此客戶。");
                }
            }
            else
            {
                Console.Write("請輸入正確的數字!");
            }
            Console.WriteLine();
        }
        public void AddCustomer()
        {
            Console.Write("請輸入客戶姓名：");
            Customer? newCustomer = new Customer();
            newCustomer.Name = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶電話：");
            newCustomer.Phone = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶城市：");
            newCustomer.City = Console.ReadLine() ?? "";

            bool isAdded = customerService.AddCustomer(newCustomer);
            if (isAdded)
            {
                Console.WriteLine("新增成功！");
            }
            else
            {
                Console.WriteLine("客戶姓名不能為空！");
            }
        }
        public void UpdateCustomer()
        {
            int? searchId = InputHelper.GetIntInput("請輸入客戶 ID：");
            if (searchId != null)
            {
                Customer? customer = customerService.FindCustomerById(searchId.Value);
                if (customer != null)
                {
                    ShowCustomer(customer);

                    string? newName = InputHelper.GetUpdateValue("客戶姓名", customer.Name);

                    string? newPhone = InputHelper.GetUpdateValue("客戶電話", customer.Phone);

                    string? newCity = InputHelper.GetUpdateValue("客戶城市", customer.City);

                    customerService.UpdateCustomerInformation(customer, newName, newPhone, newCity);

                    Console.WriteLine("客戶資料修改成功！");
                }
                else
                {
                    Console.WriteLine("找不到此客戶。");
                }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字!");
            }
        }
        public void DeleteCustomer()
        {
            int? deleteId = InputHelper.GetIntInput("請輸入要刪除的客戶 ID：");
            if (deleteId != null)
            {
                Customer? customer = customerService.FindCustomerById(deleteId.Value);

                if (customer == null)
                {
                    Console.WriteLine("找不到此客戶。");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("找到客戶：");
                    ShowCustomer(customer);

                    Console.WriteLine();
                    Console.Write("確定要刪除嗎？(Y/N)：");
                    string? confirm = Console.ReadLine();

                    if (confirm?.ToUpper() == "Y")
                    {
                        if (customerService.DeleteCustomer(customer))
                        {
                            Console.WriteLine("客戶刪除成功！");
                        }
                        else
                        {
                            Console.WriteLine("刪除客戶失敗。");
                        }
                    }
                    else
                    {
                        Console.WriteLine("已取消刪除。");
                    }
                }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字!");
            }
        }
    }
}