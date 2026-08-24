using MySqlConnector;

namespace FarmersMarketManagementSystem.Data
{
    internal class DatabaseConnection
    {
        private readonly string connectionString =
            "Server=127.0.0.1;Port=3306;Database=farmers_market_management;User ID=root;Password=admin;";

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