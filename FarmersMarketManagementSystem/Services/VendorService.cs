using FarmersMarketManagementSystem.Data;
using FarmersMarketManagementSystem.Models;
using MySqlConnector;

namespace FarmersMarketManagementSystem.Services
{
    internal class VendorService : IVendorService
    {
        private readonly DatabaseConnection databaseConnection;
        public VendorService(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
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

            using MySqlConnection connection =
            databaseConnection.CreateConnection();

            connection.Open();

            string checkBoothSql = """
                                    SELECT COUNT(*)
                                    FROM Vendors
                                    WHERE BoothNumber = @BoothNumber;
                                    """;

            MySqlCommand checkBoothCommand =
                new MySqlCommand(checkBoothSql, connection);

            checkBoothCommand.Parameters.AddWithValue("@BoothNumber", vendor.BoothNumber);

            int boothCount = Convert.ToInt32(checkBoothCommand.ExecuteScalar());

            if (boothCount > 0)
            {
                message = "攤商編號已存在。";
                return false;
            }

            string checkNameSql = """
                                    SELECT COUNT(*)
                                    FROM Vendors
                                    WHERE FirstName = @FirstName AND LastName = @LastName;
                                    """;

            MySqlCommand checkNameCommand =
            new MySqlCommand(checkNameSql, connection);

            checkNameCommand.Parameters.AddWithValue("@FirstName",vendor.FirstName);
            checkNameCommand.Parameters.AddWithValue("@LastName",vendor.LastName);

            int nameCount = Convert.ToInt32(checkNameCommand.ExecuteScalar());

            if (nameCount > 0)
            {
                message = "攤商姓名已存在。";
                return false;
            }

            vendor.Status = VendorStatus.Pending;

            string sql = """
                        INSERT INTO Vendors
                            (FirstName, LastName, Phone, BoothNumber, Status)
                        VALUES
                            (@FirstName, @LastName, @Phone, @BoothNumber, @Status);
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@FirstName", vendor.FirstName);
            command.Parameters.AddWithValue("@LastName", vendor.LastName);
            command.Parameters.AddWithValue("@Phone", vendor.Phone);
            command.Parameters.AddWithValue("@BoothNumber", vendor.BoothNumber);
            command.Parameters.AddWithValue("@Status", (int)vendor.Status);

            int affectedRows = command.ExecuteNonQuery();

            if (affectedRows > 0)
            {
                message = "攤商新增成功。";
                return true;
            }

            message = "攤商新增失敗。";
            return false;
        }
        public Vendor? GetVendor(int id)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, BoothNumber, Status
                        FROM Vendors
                        WHERE Id = @Id;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                Vendor vendor = MapVendor(reader);
                return vendor;
            }

            return null;
        }
        public bool UpdateVendor(int id, string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            Vendor? vendor = GetVendor(id);

            if (vendor == null)
            {
                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                            UPDATE Vendors
                            SET FirstName = @FirstName, LastName = @LastName
                            WHERE Id = @Id;
                         """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@Id", id);
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
        }
        public bool UpdateVendorStatus(int id, VendorStatus newStatus)
        {
            Vendor? vendor = GetVendor(id);

            if (vendor == null)
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(VendorStatus), newStatus))
            {
                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                            UPDATE Vendors
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
        public List<Vendor> GetActiveVendors()
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, BoothNumber, Status
                        FROM Vendors
                        WHERE Status = @Status;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Status", (int)VendorStatus.Active);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Vendor> activeVendors = new List<Vendor>();

            while (reader.Read())
            {
                Vendor vendor = MapVendor(reader);
                activeVendors.Add(vendor);
            }
            return activeVendors;
        }
        public List<Vendor> GetVendorsByStatus(VendorStatus status)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, BoothNumber, Status
                        FROM Vendors
                        WHERE Status = @Status;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Status", (int)status);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Vendor> filteredVendors = new List<Vendor>();

            while (reader.Read())
            {
                Vendor vendor = MapVendor(reader);
                filteredVendors.Add(vendor);
            }
            return filteredVendors;
        }
        private Vendor MapVendor(MySqlDataReader reader)
        {
            Vendor vendor = new Vendor();

            vendor.Id = reader.GetInt32("Id");
            vendor.FirstName = reader.GetString("FirstName");
            vendor.LastName = reader.GetString("LastName");
            vendor.Phone = reader.GetString("Phone");
            vendor.BoothNumber = reader.GetString("BoothNumber");
            vendor.Status = (VendorStatus)reader.GetInt32("Status");

            return vendor;
        }
        public bool DeactivateVendor(int id)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                            UPDATE Vendors
                            SET Status = @Status
                            WHERE Id = @Id;
                            """;
             
            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", (int)VendorStatus.Inactive);
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows > 0;
        }
        public List<Vendor> GetAllVendors()
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, FirstName, LastName, Phone, BoothNumber, Status
                        FROM Vendors
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Vendor> vendors = new List<Vendor>();

            while (reader.Read())
            {
                Vendor getvendor = MapVendor(reader);
                vendors.Add(getvendor);
            }
                return vendors;
        }
    }
}
