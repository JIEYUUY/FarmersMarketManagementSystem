using FarmersMarketManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FarmersMarketManagementSystem.Services
{
    internal interface IVendorService
    {
        bool AddVendor(Vendor vendor);

        Vendor? GetVendor(int id);

        List<Vendor> GetActiveVendors();

        bool UpdateVendor(int id, string firstName, string lastName);

        bool UpdateVendorStatus(int id, VendorStatus newStatus);
    }
}
