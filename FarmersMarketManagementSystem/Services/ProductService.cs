using FarmersMarketManagementSystem.Data;
using FarmersMarketManagementSystem.Models;
using MySqlConnector;

namespace FarmersMarketManagementSystem.Services
{
    internal class ProductService : IProductService
    {
        private readonly DatabaseConnection databaseConnection;
        public ProductService(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
        private Product MapProduct(MySqlDataReader reader)
        {
            Product product = new Product();

            product.Id = reader.GetInt32("Id");
            product.VendorId = reader.GetInt32("VendorId");
            product.ProductName = reader.GetString("ProductName");
            product.Category = reader.GetString("Category");
            product.Quantity = reader.GetInt32("Quantity");
            product.UnitPrice = reader.GetDecimal("UnitPrice");

            return product;
        }
        public Product? FindProductById(int id)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, VendorId, ProductName, Category, Quantity, UnitPrice
                        FROM Products
                        WHERE Id = @Id;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                Product product = MapProduct(reader);
                return product;
            }

            return null;
        }
        public bool AddProduct(Product product, out string message)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                message = "商品名稱不能為空。";
                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        INSERT INTO Products
                            (VendorId, ProductName, Category, Quantity, UnitPrice)
                        VALUES
                            (@VendorId, @ProductName, @Category, @Quantity, @UnitPrice);
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@VendorId", product.VendorId);
            command.Parameters.AddWithValue("@ProductName", product.ProductName);
            command.Parameters.AddWithValue("@Category", product.Category);
            command.Parameters.AddWithValue("@Quantity", product.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);

            try
            {
                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    message = "商品新增成功。";
                    return true;
                }

                message = "商品新增失敗。";
                return false;
            }
            catch (MySqlException)
            {
                message = "發生資料庫錯誤。";
                return false;
            }
        }
        public List<Product> GetAllProducts()
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, VendorId, ProductName, Category, Quantity, UnitPrice
                        FROM Products;
                        """;
            MySqlCommand command =
                new MySqlCommand(sql, connection);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Product> products = new List<Product>();

            while (reader.Read())
            {
                Product product = MapProduct(reader);
                products.Add(product);
            }
            return products;
        }
        public bool UpdateProductInformation(Product product, int? newVendorId, string? newCategory, string? newProductName, int? newQuantity, decimal? newUnitPrice)
        {
            if (newVendorId.HasValue)
            {
                product.VendorId = newVendorId.Value;
            }
            if (!string.IsNullOrWhiteSpace(newProductName))
            {
                product.ProductName = newProductName;
            }
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                product.Category = newCategory;
            }
            if (newQuantity.HasValue)
            {
                product.Quantity = newQuantity.Value;
            }
            if (newUnitPrice.HasValue)
            {
                product.UnitPrice = newUnitPrice.Value;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        UPDATE Products
                        SET VendorId = @VendorId,
                            ProductName = @ProductName,
                            Category = @Category,
                            Quantity = @Quantity,
                            UnitPrice = @UnitPrice
                        WHERE Id = @Id;
                        """;
            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@VendorId", product.VendorId);
            command.Parameters.AddWithValue("@ProductName", product.ProductName);
            command.Parameters.AddWithValue("@Category", product.Category);
            command.Parameters.AddWithValue("@Quantity", product.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
            command.Parameters.AddWithValue("@Id", product.Id);

            int affectedRows = command.ExecuteNonQuery();

            return affectedRows > 0;
        }
    }
}
