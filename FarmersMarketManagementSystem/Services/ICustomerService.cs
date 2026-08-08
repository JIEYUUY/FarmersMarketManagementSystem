using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal interface ICustomerService
    {
        Customer? FindCustomerById(int id);
        bool AddCustomer(Customer customer);

        List<Customer> GetAllCustomers();

        void UpdateCustomerInformation(Customer customer, string? newName, string? newPhone, string? newCity);

        bool DeleteCustomer(Customer customer);
    }
}