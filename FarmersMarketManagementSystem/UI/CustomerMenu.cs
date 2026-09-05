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
            Console.WriteLine($"姓名：{customer.FirstName} {customer.LastName}");
            Console.WriteLine($"電話：{customer.Phone}");
            Console.WriteLine($"城市：{customer.City}");
            Console.WriteLine($"狀態：{customer.Status}");
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
        5. 停用客戶
        6. 更新客戶狀態
        7. 依姓名搜尋客戶
        8. 依城市搜尋客戶

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
                        DeactivateCustomer();
                        break;

                    case "6":
                        UpdateCustomerStatus();
                        break;

                    case "7":
                        SearchCustomersByName();
                        break;

                    case "8":
                        SearchCustomersByCity();
                        break;

                    case "0":
                        isCustomerMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～8。");
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
                Customer? customer = customerService.GetCustomer(searchId.Value);

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
            Customer? newCustomer = new Customer();

            Console.Write("請輸入客戶姓名：");
            newCustomer.FirstName = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶姓氏：");
            newCustomer.LastName = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶電話：");
            newCustomer.Phone = Console.ReadLine() ?? "";

            Console.Write("請輸入客戶城市：");
            newCustomer.City = Console.ReadLine() ?? "";

            bool isAdded = customerService.AddCustomer(newCustomer, out string message);
            if (isAdded)
            {
                Console.WriteLine("新增成功！");
            }
            else
            {
                Console.WriteLine(message);
            }
        }
        public void UpdateCustomer()
        {
            int? searchId = InputHelper.GetIntInput("請輸入客戶 ID：");

            if (searchId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Customer? customer = customerService.GetCustomer(searchId.Value);

            if (customer == null)
            {
                Console.WriteLine("找不到此客戶。");
                return;
            }

            ShowCustomer(customer);

            string? newFirstName =
                InputHelper.GetUpdateValue("客戶名字", customer.FirstName);

            string? newLastName =
                InputHelper.GetUpdateValue("客戶姓氏", customer.LastName);

            string? newPhone =
                InputHelper.GetUpdateValue("客戶電話", customer.Phone);

            string? newCity =
                InputHelper.GetUpdateValue("客戶城市", customer.City);

            bool isUpdated = customerService.UpdateCustomerInformation(
                customer.Id,
                newFirstName ?? customer.FirstName,
                newLastName ?? customer.LastName,
                newPhone ?? customer.Phone,
                newCity ?? customer.City
                
            );

            if (isUpdated)
            {
                Console.WriteLine("客戶資料修改成功！");
            }
            else
            {
                Console.WriteLine("修改客戶失敗。");
            }
        }

        public void UpdateCustomerStatus()
        {
            int? customerId =
                InputHelper.GetIntInput("請輸入要更新狀態的客戶 ID：");

            if (customerId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Customer? customer = customerService.GetCustomer(customerId.Value);

            if (customer == null)
            {
                Console.WriteLine("找不到此客戶。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("找到客戶：");
            ShowCustomer(customer);

            Console.Write("請輸入要更新的狀態：(1.Pending 2.Active 3.Suspended 4.Banned 5.Inactive)：");
            string? statusInput = Console.ReadLine();

            if (!int.TryParse(statusInput, out int statusValue))
            {
                Console.WriteLine("請輸入有效的狀態代碼。");
                return;
            }

            if (!Enum.IsDefined(typeof(CustomerStatus), statusValue))
            {
                Console.WriteLine("請輸入 1～5 的有效狀態代碼。");
                return;
            }

            CustomerStatus newStatus = (CustomerStatus)statusValue;

            if (customerService.UpdateCustomerStatus(customer.Id, newStatus))
            {
                Console.WriteLine("客戶狀態更新成功！");
            }
            else
            {
                Console.WriteLine("更新客戶狀態失敗。");
            }
        }
        public void DeactivateCustomer()
        {
            int? customerId =
                InputHelper.GetIntInput("請輸入要停用的客戶 ID：");

            if (customerId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Customer? customer = customerService.GetCustomer(customerId.Value);

            if (customer == null)
            {
                Console.WriteLine("找不到此客戶。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("找到客戶：");
            ShowCustomer(customer);

            Console.Write("確定要停用此客戶嗎？(Y/N)：");
            string? confirmInput = Console.ReadLine();

            if (confirmInput?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("停用已取消。");
                return;
            }

            if (customerService.DeactivateCustomer(customer.Id))
            {
                Console.WriteLine("停用客戶成功！");
            }
            else
            {
                Console.WriteLine("停用客戶失敗。");
            }
        }
        public void SearchCustomersByName()
        {
            Console.Write("請輸入要搜尋的客戶姓名：");
            string? keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("請輸入有效的姓名。");
                return;
            }

            List<Customer> customers = customerService.SearchCustomersByName(keyword);

            if (customers.Any())
            {
                Console.WriteLine("搜尋結果：");
                foreach (Customer customer in customers)
                {
                    ShowCustomer(customer);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("找不到符合條件的客戶。");
            }
        }
        public void SearchCustomersByCity()
        {
            Console.Write("請輸入要搜尋的客戶城市：");
            string? keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("請輸入有效的城市名稱。");
                return;
            }

            List<Customer> customers = customerService.SearchCustomersByCity(keyword);

            if (customers.Any())
            {
                Console.WriteLine("搜尋結果：");
                foreach (Customer customer in customers)
                {
                    ShowCustomer(customer);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("找不到符合條件的客戶。");
            }
        }
    }
}
