using FarmersMarketManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IVendorService
    {
        Vendor? FindVendorById(int id);

        bool AddVendor(Vendor vendor);

        List<Vendor> GetAllVendors();

        void UpdateVendorInformation(
            Vendor vendor,
            string? firstName,
            string? lastName,
            string? phone,
            string? boothNumber);

        bool DeactivateVendor(Vendor vendor);

        bool ActivateVendor(Vendor vendor);
    }
}
