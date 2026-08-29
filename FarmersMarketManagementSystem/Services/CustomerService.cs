using FarmersMarketManagementSystem.Data;
using FarmersMarketManagementSystem.Models;
using MySqlConnector;

namespace FarmersMarketManagementSystem.Services
{
    internal class CustomerService : ICustomerService
    {
        private readonly DatabaseConnection databaseConnection;
        public CustomerService(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
        private Customer MapCustomer(MySqlDataReader reader)
        {
            Customer customer = new Customer();

            customer.Id = reader.GetInt32("Id");
            customer.FirstName = reader.GetString("FirstName");
            customer.LastName = reader.GetString("LastName");
            customer.Phone = reader.GetString("Phone");
            customer.City = reader.GetString("City");
            customer.Status = (CustomerStatus)reader.GetInt32("Status");

            return customer;
        }
        public Customer? GetCustomer(int id)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, City, Status
                        FROM Customers
                        WHERE Id = @Id;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                Customer customer = MapCustomer(reader);
                return customer;
            }

            return null;
        }
        public bool AddCustomer(Customer customer, out string message)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                message = "名字不能為空。";
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.LastName))
            {
                message = "姓氏不能為空。";
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.Phone))
            {
                message = "電話不能為空。";
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer.City))
            {
                message = "城市不能為空。";
                return false;
            }

            if (customer.Phone.Length != 10)
            {
                message = "電話必須是 10 碼。";
                return false;
            }

            foreach (char c in customer.Phone)
            {
                if (!char.IsDigit(c))
                {
                    message = "電話必須是數字。";
                    return false;
                }
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string checkPhoneSql = """
                                    SELECT COUNT(*)
                                    FROM Customers
                                    WHERE Phone = @Phone;
                                    """;

            MySqlCommand checkPhoneCommand =
                new MySqlCommand(checkPhoneSql, connection);

            checkPhoneCommand.Parameters.AddWithValue("@Phone",customer.Phone);

            int phoneCount =Convert.ToInt32(checkPhoneCommand.ExecuteScalar());

            if (phoneCount > 0)
            {
                message = "電話號碼已存在。";
                return false;
            }

            customer.Status = CustomerStatus.Pending;

            string sql = """
                        INSERT INTO Customers
                            (FirstName, LastName, Phone, City, Status)
                        VALUES
                            (@FirstName, @LastName, @Phone, @City, @Status);
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@FirstName",
                customer.FirstName
            );

            command.Parameters.AddWithValue(
                "@LastName",
                customer.LastName
            );

            command.Parameters.AddWithValue(
                "@Phone",
                customer.Phone
            );

            command.Parameters.AddWithValue(
                "@City",
                customer.City
            );

            command.Parameters.AddWithValue(
                "@Status",
                (int)customer.Status
            );

            int affectedRows =
                command.ExecuteNonQuery();

            if (affectedRows > 0)
            {
                message = "客戶新增成功。";
                return true;
            }

            message = "客戶新增失敗。";
            return false;
        }
        public List<Customer> GetAllCustomers()
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, City, Status
                        FROM Customers;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Customer> customers = new List<Customer>();

            while (reader.Read())
            {
                Customer customer = MapCustomer(reader);
                customers.Add(customer);
            }

            return customers;
        }

        public bool UpdateCustomerInformation(int id, string firstName, string lastName, string phone, string city)
        {

            Customer? customer = GetCustomer(id);

            if (customer == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.All(char.IsDigit))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string checkPhoneSql = """
                                    SELECT COUNT(*)
                                    FROM Customers
                                    WHERE Phone = @Phone AND Id != @Id;
                                    """;

            MySqlCommand checkPhoneCommand =
                new MySqlCommand(checkPhoneSql, connection);

            checkPhoneCommand.Parameters.AddWithValue("@Phone", phone);
            checkPhoneCommand.Parameters.AddWithValue("@Id", id);

            if (Convert.ToInt32(checkPhoneCommand.ExecuteScalar()) > 0)
            {
                return false;
            }

            string sql = """
                            UPDATE Customers
                            SET FirstName = @FirstName, LastName = @LastName, Phone = @Phone, City = @City
                            WHERE Id = @Id;
                         """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@Phone", phone.Trim());
            command.Parameters.AddWithValue("@City", city);
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
        }

        public bool UpdateCustomerStatus(int id, CustomerStatus newStatus)
        {
            Customer? customer = GetCustomer(id);

            if (customer == null)
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(CustomerStatus), newStatus))
            {
                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                            UPDATE Customers
                            SET Status = @Status
                            WHERE Id = @Id;
                            """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Status", (int)newStatus);
            command.Parameters.AddWithValue("@Id", id);
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
        }
        public bool DeactivateCustomer(int id)
        {
            Customer? customer = GetCustomer(id);
            
            if (customer == null) 
            { 
                return false; 
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                            UPDATE Customers
                            SET Status = @Status
                            WHERE Id = @Id;
                            """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", (int)CustomerStatus.Inactive);
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
        }
    }
}
