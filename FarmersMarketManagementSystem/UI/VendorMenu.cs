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
            Console.WriteLine($"狀態：{(vendor.IsActive ? "啟用" : "停用")}");
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
        5. 停用攤商
        6. 啟用攤商
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
                        DeactivateVendor();
                        break;

                    case "6":
                        ActivateVendor();
                        break;

                    case "0":
                        isVendorMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～6。");
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

        public void DeactivateVendor()
        {
            int? deleteId =
                InputHelper.GetIntInput("請輸入要停用的攤商 ID：");

            if (deleteId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Vendor? vendor = vendorService.GetVendor(deleteId.Value);

            if (vendor == null)
            {
                Console.WriteLine("找不到此攤商。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("找到攤商：");
            ShowVendor(vendor);

            Console.Write("確定要停用嗎？(Y/N)：");
            string? confirm = Console.ReadLine();

            if (confirm?.ToUpper() == "Y")
            {
                if (vendorService.DeactivateVendor(vendor.Id))
                {
                    Console.WriteLine("攤商停用成功！");
                }
                else
                {
                    Console.WriteLine("停用攤商失敗。");
                }
            }
            else
            {
                Console.WriteLine("已取消停用。");
            }
        }
        public void ActivateVendor()
        {
            int? deleteId =
                InputHelper.GetIntInput("請輸入要啟用的攤商 ID：");

            if (deleteId == null)
            {
                Console.WriteLine("請輸入正確的數字!");
                return;
            }

            Vendor? vendor = vendorService.GetVendor(deleteId.Value);

            if (vendor == null)
            {
                Console.WriteLine("找不到此攤商。");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("找到攤商：");
            ShowVendor(vendor);

            Console.Write("確定要啟用嗎？(Y/N)：");
            string? confirm = Console.ReadLine();

            if (confirm?.ToUpper() == "Y")
            {
                if (vendorService.ActivateVendor(vendor.Id))
                {
                    Console.WriteLine("攤商啟用成功！");
                }
                else
                {
                    Console.WriteLine("啟用攤商失敗。");
                }
            }
            else
            {
                Console.WriteLine("已取消啟用。");
            }
        }
    }
}