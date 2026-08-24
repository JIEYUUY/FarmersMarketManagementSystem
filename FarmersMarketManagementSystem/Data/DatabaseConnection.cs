using MySqlConnector;

namespace FarmersMarketManagementSystem.Data
{
    internal class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection()
        {
            string? password =Environment.GetEnvironmentVariable("FARMERS_MARKET_DB_PASSWORD");

            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException(
                    "找不到資料庫密碼環境變數 FARMERS_MARKET_DB_PASSWORD。"
                );
            }

            connectionString =
                $"Server=127.0.0.1;Port=3306;Database=farmers_market_management;User ID=root;Password={password};";
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public void TestConnection()
        {
            using MySqlConnection connection = CreateConnection();

            connection.Open();

            Console.WriteLine("MySQL 連線成功！");
        }
    }
}