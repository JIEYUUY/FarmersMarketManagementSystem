using FarmersMarketManagementSystem;
using System;

namespace FarmersMarketManagementSystem
{
    internal class Program
    {
        static List<Customer> customers = new List<Customer>();
        static int nextCustomerId = 1;
        static void Main(string[] args)
        {
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
                Console.WriteLine("4. 銷售報表");
                Console.WriteLine("5. 系統資訊");
                Console.WriteLine("0. 離開系統");
                Console.WriteLine();

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowCustomerMenu();
                        break;

                    case "2":
                        Console.WriteLine();
                        Console.WriteLine("已進入商品管理功能。");
                        break;

                    case "3":
                        Console.WriteLine();
                        Console.WriteLine("已進入攤商管理功能。");
                        break;

                    case "4":
                        Console.WriteLine();
                        Console.WriteLine("已進入銷售報表功能。");
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
        static void ShowCustomerMenu()
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
        static void ShowAllCustomers()
        {
            Console.WriteLine();
            foreach (Customer customer in customers)
            {
                ShowCustomer(customer);
            }
        }
        static void SearchCustomer()
        {
            Console.Write("請輸入要搜尋的客戶 ID：");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int searchId))
            {
                Customer? customer = FindCustomerById(searchId);

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
                Console.WriteLine("請輸入正確的數字！");
            }

            Console.WriteLine();
        }
        static void AddCustomer()
        {
            Console.Write("請輸入客戶姓名：");
            Customer? newCustomer = new Customer();
            newCustomer.Name = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶電話：");
            newCustomer.Phone = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶城市：");
            newCustomer.City = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(newCustomer.Name))
            {
                newCustomer.Id = nextCustomerId;
                nextCustomerId++;
                customers.Add(newCustomer);
                Console.WriteLine("新增成功！");
            }
            else
            {
                Console.WriteLine("客戶姓名不能為空！");
            }
        }
        static void UpdateCustomer()
        {
            Console.Write("請輸入客戶 ID：");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int searchId))
            {
                Customer? customer = FindCustomerById(searchId);
                    if (customer != null)
                    {
                        Console.WriteLine($"ID：{customer.Id}");
                        Console.WriteLine($"姓名：{customer.Name}");
                        Console.Write($"請輸入新的客戶姓名（目前：{customer.Name}，直接按 Enter 保留）：");
                        string? newName = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            customer.Name = newName;
                        }
                        Console.WriteLine($"電話：{customer.Phone}");
                        Console.Write($"請輸入新的客戶電話（目前：{customer.Phone}，直接按 Enter 保留）：");
                        string? newPhone = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newPhone))
                        {
                            customer.Phone = newPhone;
                        }
                        Console.WriteLine($"城市：{customer.City}");
                        Console.Write($"請輸入新的客戶城市（目前：{customer.City}，直接按 Enter 保留）：");
                        string? newCity = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(newCity))
                        {
                            customer.City = newCity;
                        }

                        Console.WriteLine("客戶資料修改成功！");
                    }
                    else
                    {
                        Console.WriteLine("找不到此客戶。");
                    }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字！");
            }
        }
        static void DeleteCustomer()
        {
            Console.Write("請輸入要刪除的客戶 ID：");
            string? deleteInput = Console.ReadLine();

            if (int.TryParse(deleteInput, out int deleteId))
            {
                Customer? customer = FindCustomerById(deleteId);

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
                        customers.Remove(customer);
                        Console.WriteLine("客戶刪除成功！");
                    }
                    else
                    {
                        Console.WriteLine("已取消刪除。");
                    }
                }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字！");
            }

        }
        static Customer? FindCustomerById(int id)
        {
            foreach (Customer customer in customers)
            {
                if (customer.Id == id)
                {
                    return customer;
                }
            }

            return null;
        }
        static void ShowCustomer(Customer customer)
        {
            Console.WriteLine($"ID：{customer.Id}");
            Console.WriteLine($"姓名：{customer.Name}");
            Console.WriteLine($"電話：{customer.Phone}");
            Console.WriteLine($"城市：{customer.City}");
        }
    }
}


