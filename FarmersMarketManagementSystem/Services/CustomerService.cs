using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using FarmersMarketManagementSystem.Models;

namespace FarmersMarketManagementSystem.Services
{
    internal class CustomerService
    {
        private List<Customer> customers = new List<Customer>();
        private int nextCustomerId = 1;

        public Customer? FindCustomerById(int id)
        {
            foreach (Customer customer in customers)
            {
                if (customer.Id == id)
                { 
                    return customer;
                }
            }

            return null;
        }

        public bool AddCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                return false;
            }

            customer.Id = nextCustomerId;
            nextCustomerId++;

            customers.Add(customer);

            return true;
        }
        public List<Customer> GetAllCustomers()
        {
            return customers;
        }

        public void UpdateCustomerInformation(Customer customer, string? newName, string? newPhone, string? newCity)
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                customer.Name = newName;
            }
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                customer.Phone = newPhone;
            }
            if (!string.IsNullOrWhiteSpace(newCity))
            {
                customer.City = newCity;
            }
        }

        public bool DeleteCustomer(Customer customer)
        {
            return customers.Remove(customer);
        }
    }
}
