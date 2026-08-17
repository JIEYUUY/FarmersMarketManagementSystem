using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class VendorService : IVendorService
    {
        private List<Vendor> vendors = new List<Vendor>();
        private int nextVendorId = 1;
        public bool AddVendor(Vendor vendor)
        {
            if (string.IsNullOrEmpty(vendor.FirstName) ||
                string.IsNullOrEmpty(vendor.LastName))
            {
                return false;
            }

            foreach (Vendor existingVendor in vendors)
            {
                if (existingVendor.FirstName == vendor.FirstName &&
                    existingVendor.LastName == vendor.LastName)
                {
                    return false;
                }
            }

            vendor.Id = nextVendorId++;
            vendor.Status = VendorStatus.Pending;
            vendors.Add(vendor);

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
    }
}
