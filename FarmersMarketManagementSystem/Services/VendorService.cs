using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class VendorService : IVendorService
    {
        private List<Vendor> vendors = new List<Vendor>();
        private int nextVendorId = 1;
        public Vendor? FindVendorById(int id)
        {
            foreach(Vendor vendor in vendors)
            {
                if (vendor.Id == id)
                {
                    return vendor;
                }
            }
            return null;
        }
    }
