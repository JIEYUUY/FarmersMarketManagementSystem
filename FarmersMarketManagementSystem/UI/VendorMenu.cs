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
            Console.WriteLine($"電話：{vendor.Phone}");
            Console.WriteLine($"編號：{vendor.BoothNumber}");
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
        2. 依狀態篩選攤商
        3. 搜尋攤商
        4. 新增攤商
        5. 修改攤商
        6. 更新攤商狀態
        7. 停用攤商
        8. 顯示所有攤商
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
                        ShowVendorsByStatus();
                        break;

                    case "3":
                        SearchVendor();
                        break;

                    case "4":
                        AddVendor();
                        break;

                    case "5":
                        UpdateVendor();
                        break;

                    case "6":
                        UpdateVendorStatus();
                        break;

                    case "7":
                        DeactivateVendor();
                        break;

                    case "8":
                        GetAllVendors();
                        break;

                    case "0":
                        isVendorMenuRunning = false;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("輸入錯誤，請輸入 0～8。");
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

            Console.Write("請輸入攤商姓名：");
            newVendor.FirstName = Console.ReadLine() ?? "";

            Console.Write("請輸入攤商姓氏：");
            newVendor.LastName = Console.ReadLine() ?? "";

            Console.Write("請輸入攤商電話：");
            newVendor.Phone = Console.ReadLine() ?? "";

            Console.Write("請輸入攤商編號：");
            newVendor.BoothNumber = Console.ReadLine() ?? "";

            bool isAdded = vendorService.AddVendor(newVendor, out string message);

            if (isAdded)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine($"新增失敗：{message}");
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

            Console.Write("請輸入要更新的狀態：(1.Pending 2.Active 3.Suspended 4.Banned 5.Inactive)：");
            string? statusInput = Console.ReadLine();

            if (!int.TryParse(statusInput, out int statusValue))
            {
                Console.WriteLine("請輸入有效的狀態代碼。");
                return;
            }

            if (!Enum.IsDefined(typeof(VendorStatus), statusValue))
            {
                Console.WriteLine("請輸入 1～5 的有效狀態代碼。");
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
        public void ShowVendorsByStatus()
        {
            int? searchStatus = InputHelper.GetIntInput("請輸入要查詢的狀態：(1.Pending 2.Active 3.Suspended 4.Banned 5.Inactive)：");

            if (searchStatus == null)
            {
                Console.WriteLine("請輸入有效的狀態代碼。");
                return;
            }

            if (!Enum.IsDefined(typeof(VendorStatus), searchStatus.Value))
            {
                Console.WriteLine("請輸入 1～5 的有效狀態代碼。");
                return;
            }

            VendorStatus status = (VendorStatus)searchStatus.Value;
            var vendors = vendorService.GetVendorsByStatus(status);

            if (vendors.Count == 0)
            {
                Console.WriteLine($"找不到狀態為 {status} 的攤商。");
                return;
            }
            foreach (var vendor in vendors)
            {
                ShowVendor(vendor);
            }
        }
        public void DeactivateVendor()
        {
            int? vendorId =
                InputHelper.GetIntInput("請輸入要停用的攤商 ID：");

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

            Console.Write("確定要停用此攤商嗎？(Y/N)：");
            string? confirmInput = Console.ReadLine();

            if (confirmInput?.Trim().ToUpper() != "Y")
            {
                Console.WriteLine("停用已取消。");
                return;
            }

            if (vendorService.DeactivateVendor(vendor.Id))
            {
                Console.WriteLine("停用攤商成功！");
            }
            else
            {
                Console.WriteLine("停用攤商失敗。");
            }
        }
        public void GetAllVendors()
        {
            List<Vendor> vendors = vendorService.GetAllVendors();

            if (vendors.Count == 0)
            {
                Console.WriteLine("找不到任何攤商。");
                return;
            }

            foreach (var vendor in vendors)
            {
                ShowVendor(vendor);
            }
        }
    }
}