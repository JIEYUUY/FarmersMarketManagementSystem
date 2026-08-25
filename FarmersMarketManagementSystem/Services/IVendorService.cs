using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IVendorService
    {
        bool AddVendor(Vendor vendor, out string message);

        Vendor? GetVendor(int id);

        List<Vendor> GetActiveVendors();

        bool UpdateVendor(int id, string firstName, string lastName);

        bool UpdateVendorStatus(int id, VendorStatus newStatus);

        List<Vendor> GetVendorsByStatus(VendorStatus status);

        bool DeactivateVendor(int id);
        List<Vendor> GetAllVendors();
    }
}
