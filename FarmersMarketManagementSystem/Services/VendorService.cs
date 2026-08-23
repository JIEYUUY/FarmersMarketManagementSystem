using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class VendorService : IVendorService
    {
        private List<Vendor> vendors = new List<Vendor>();
        private int nextVendorId = 1;
        public bool AddVendor(Vendor vendor, out string message)
        {
            if (string.IsNullOrEmpty(vendor.FirstName) ||
                string.IsNullOrEmpty(vendor.LastName))
            {
                message = "姓名不能為空。";
                return false;
            }

            if (string.IsNullOrEmpty(vendor.Phone))
            {
                message = "電話不能為空。";
                return false;
            }

            if (string.IsNullOrEmpty(vendor.BoothNumber))
            {
                message = "攤商編號不能為空。";
                return false;
            }

            if (vendor.Phone.Length != 10)
            {
                message = "電話必須是 10 碼。";
                return false;
            }

            foreach (char c in vendor.Phone)
            {
                if (!char.IsDigit(c))
                {
                    message = "電話必須是數字。";
                    return false;
                }
            }

            foreach (Vendor existingVendor in vendors)
            {
                if (existingVendor.FirstName == vendor.FirstName &&
                    existingVendor.LastName == vendor.LastName)
                {
                    message = "攤商姓名已存在。";
                    return false;
                }

                if (existingVendor.BoothNumber == vendor.BoothNumber)
                {
                    message = "攤商編號已存在。";
                    return false;
                }
            }

            vendor.Id = nextVendorId++;
            vendor.Status = VendorStatus.Pending;
            vendors.Add(vendor);

            message = "攤商新增成功。";
            return true;
        }
        public Vendor? GetVendor(int id)
        {
            foreach (Vendor vendor in vendors)
            {
                if (vendor.Id == id)
                {
                    return vendor;
                }
            }
            return null;
        }
        public bool UpdateVendor(int id, string firstName, string lastName)
        {
            Vendor? vendor = GetVendor(id);

            if (vendor != null &&
                !string.IsNullOrEmpty(firstName) &&
                !string.IsNullOrEmpty(lastName))
            {
                vendor.FirstName = firstName;
                vendor.LastName = lastName;

                return true;
            }

            return false;
        }
        public bool UpdateVendorStatus(int id, VendorStatus newStatus)
        {
            Vendor? vendor = GetVendor(id);

            if (vendor == null)
            {
                return false;
            }

            if (!Enum.IsDefined(newStatus))
            {
                return false;
            }

            vendor.Status = newStatus;
            return true;
        }
        public List<Vendor> GetActiveVendors()
        {
            List<Vendor> activeVendors = new List<Vendor>();
            foreach (Vendor vendor in vendors)
            {
                if (vendor.Status == VendorStatus.Active)
                {
                    activeVendors.Add(vendor);
                }
            }
            return activeVendors;
        }
        public List<Vendor> GetVendorsByStatus(VendorStatus status)
        {
            List<Vendor> filteredVendors = new List<Vendor>();
            foreach (Vendor vendor in vendors)
            {
                if (vendor.Status == status)
                {
                    filteredVendors.Add(vendor);
                }
            }
            return filteredVendors;
        }
    }
}
