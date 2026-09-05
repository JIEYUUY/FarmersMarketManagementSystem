using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface ICustomerService
    {
        Customer? GetCustomer(int id);
        bool AddCustomer(Customer customer, out string message);

        List<Customer> GetAllCustomers();

        bool UpdateCustomerInformation(int id, string firstName, string lastName, string phone, string city);

        bool UpdateCustomerStatus(int id, CustomerStatus newStatus);

        bool DeactivateCustomer(int id);
        List<Customer> SearchCustomersByName(string keyword);
        List<Customer> SearchCustomersByCity(string keyword);

    }
}
