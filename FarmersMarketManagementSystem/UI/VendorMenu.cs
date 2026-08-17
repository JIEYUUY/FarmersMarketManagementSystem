using FarmersMarketManagementSystem.Models;
using FarmersMarketManagementSystem.Services;
using FarmersMarketManagementSystem.Utilities;

namespace FarmersMarketManagementSystem.UI
{
    internal class VendorMenu
    {
        private readonly IVendorService vendorService;

        public VendorMenu(IVendorService vendorService)
        {
            this.vendorService = vendorService;
        }

        public void ShowVendor(Vendor vendor)
        {
            Console.WriteLine($"ID：{vendor.Id}");
            Console.WriteLine($"姓名：{vendor.FirstName} {vendor.LastName}");
            Console.WriteLine($"狀態：{vendor.Status}");
            Console.WriteLine();
        }

        public void ShowActiveVendors()
        {
            Console.WriteLine();

            List<Vendor> activeVendors = vendorService.GetActiveVendors();

            foreach (Vendor vendor in activeVendors)
            {
                ShowVendor(vendor);
            }
        }

        public void ShowVendorMenu()
        {
            bool isVendorMenuRunning = true;

            while (isVendorMenuRunning)
            {
                Console.Clear();

                Console.WriteLine("""
        ====================
         攤商管理
        ====================

        1. 顯示啟用中的攤商
        2. 搜尋攤商
        3. 新增攤商
        4. 修改攤商
        5. 更新攤商狀態
        0. 返回主選單

        """);

                Console.Write("請選擇功能：");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowActiveVendors();
                        break;

                    case "2":
                        SearchVendor();
                        break;

                    case "3":
                        AddVendor();
                        break;

                    case "4":
                        UpdateVendor();
                        break;

                    case "5":
                        UpdateVendorStatus();
                        break;

                    case "0":
                        isVendorMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～5。");
                        break;
                }

                if (isVendorMenuRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("按任意鍵繼續...");
                    Console.ReadKey();
                }
            }
        }

        public void SearchVendor()
        {
            int? searchId = InputHelper.GetIntInput("請輸入攤商 ID：");

            if (searchId != null)
            {
                Vendor? vendor = vendorService.GetVendor(searchId.Value);

                if (vendor != null)
                {
                    ShowVendor(vendor);
                }
                else
                {
                    Console.WriteLine("找不到此攤商。");
                }
            }
            else
            {
                Console.WriteLine("請輸入正確的數字!");
            }
        }

        public void AddVendor()
        {
            Vendor newVendor = new Vendor();

            Console.Write("請輸入攤商名字：");
            newVendor.FirstName = Console.ReadLine() ?? "";

            Console.Write("請輸入攤商姓氏：");
            newVendor.LastName = Console.ReadLine() ?? "";

            bool isAdded = vendorService.AddVendor(newVendor);

            if (isAdded)
            {
                Console.WriteLine("新增攤商成功！");
            }
            else
            {
                Console.WriteLine("新增失敗，姓名不能為空或攤商已存在。");
            }
        }

        public void UpdateVendor()
        {
            int? searchId = InputHelper.GetIntInput("請輸入攤商 ID：");

            if (searchId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Vendor? vendor = vendorService.GetVendor(searchId.Value);

            if (vendor == null)
            {
                Console.WriteLine("找不到此攤商。");
                return;
            }

            ShowVendor(vendor);

            string? newFirstName =
                InputHelper.GetUpdateValue("攤商名字", vendor.FirstName);

            string? newLastName =
                InputHelper.GetUpdateValue("攤商姓氏", vendor.LastName);

            bool isUpdated = vendorService.UpdateVendor(
                vendor.Id,
                newFirstName ?? vendor.FirstName,
                newLastName ?? vendor.LastName
            );

            if (isUpdated)
            {
                Console.WriteLine("攤商資料修改成功！");
            }
            else
            {
                Console.WriteLine("修改攤商失敗。");
            }
        }

        public void UpdateVendorStatus()
        {
            int? vendorId =
                InputHelper.GetIntInput("請輸入要更新狀態的攤商 ID：");

            if (vendorId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Vendor? vendor = vendorService.GetVendor(vendorId.Value);

            if (vendor == null)
            {
                Console.WriteLine("找不到此攤商。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("找到攤商：");
            ShowVendor(vendor);

            Console.Write("請輸入要更新的狀態：(1.Pending 2.Active 3.Suspended 4.Banned)：");
            string? statusInput = Console.ReadLine();

            if (!int.TryParse(statusInput, out int statusValue))
            {
                Console.WriteLine("請輸入有效的狀態代碼。");
                return;
            }

            if (!Enum.IsDefined(typeof(VendorStatus), statusValue))
            {
                Console.WriteLine("請輸入 1～4 的有效狀態代碼。");
                return;
            }

            VendorStatus newStatus = (VendorStatus)statusValue;

            if (vendorService.UpdateVendorStatus(vendor.Id, newStatus))
            {
                Console.WriteLine("攤商狀態更新成功！");
            }
            else
            {
                Console.WriteLine("更新攤商狀態失敗。");
            }
        }
    }
}