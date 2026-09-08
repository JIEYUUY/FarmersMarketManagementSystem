using FarmersMarketManagementSystem.Data;
using FarmersMarketManagementSystem.Models;
using MySqlConnector;

namespace FarmersMarketManagementSystem.Services
{
    internal class OrderService : IOrderService
    {
        private readonly IProductService productService;
        private readonly DatabaseConnection databaseConnection;

        public OrderService(
            IProductService productService,
            DatabaseConnection databaseConnection)
        {
            this.productService = productService;
            this.databaseConnection = databaseConnection;
        }

        public bool AddItemToOrder(
            Order order,
            int productId,
            int quantity,
            out string message)
        {
            Product? product =
                productService.FindProductById(productId);

            if (product == null)
            {
                message = "商品不存在。";
                return false;
            }

            if (quantity <= 0)
            {
                message = "購買數量必須大於 0。";
                return false;
            }

            if (product.Quantity < quantity)
            {
                message = "庫存不足。";
                return false;
            }

            OrderItem? existingItem =
                order.Items.FirstOrDefault(
                    item => item.ProductId == productId
                );

            if (existingItem != null)
            {
                int newQuantity =
                    existingItem.Quantity + quantity;

                if (newQuantity > product.Quantity)
                {
                    message = "庫存不足，無法加入更多商品。";
                    return false;
                }

                existingItem.Quantity = newQuantity;

                message = "商品數量已更新。";
                return true;
            }

            OrderItem item = new OrderItem();

            item.ProductId = product.Id;
            item.Quantity = quantity;
            item.UnitPrice = product.UnitPrice;

            order.Items.Add(item);

            message = "商品已加入訂單。";
            return true;
        }

        public bool CreateOrder(Order order, out string message)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                //計算訂單總價
                order.TotalPrice = order.CalculateTotalPrice();


                //新增 Orders
                string orderSql = """
            INSERT INTO Orders
                (CustomerId, OrderDate, TotalPrice)
            VALUES
                (@CustomerId, @OrderDate, @TotalPrice);
            """;

                MySqlCommand orderCommand =
                    new MySqlCommand(orderSql, connection, transaction);

                orderCommand.Parameters.AddWithValue(
                    "@CustomerId",
                    order.CustomerId);

                orderCommand.Parameters.AddWithValue(
                    "@OrderDate",
                    order.OrderDate);

                orderCommand.Parameters.AddWithValue(
                    "@TotalPrice",
                    order.TotalPrice);

                orderCommand.ExecuteNonQuery();


                //取得 MySQL 自動產生的 Order Id
                order.Id = (int)orderCommand.LastInsertedId;


                //把每一個 OrderItem 寫進 OrderItems
                foreach (OrderItem item in order.Items)
                {
                    string itemSql = """
                                    INSERT INTO OrderItems
                                        (OrderId, ProductId, Quantity, UnitPrice)
                                    VALUES
                                        (@OrderId, @ProductId, @Quantity, @UnitPrice);
                                    """;

                    MySqlCommand itemCommand =
                        new MySqlCommand(
                            itemSql,
                            connection,
                            transaction);

                    itemCommand.Parameters.AddWithValue(
                        "@OrderId",
                        order.Id);

                    itemCommand.Parameters.AddWithValue(
                        "@ProductId",
                        item.ProductId);

                    itemCommand.Parameters.AddWithValue(
                        "@Quantity",
                        item.Quantity);

                    itemCommand.Parameters.AddWithValue(
                        "@UnitPrice",
                        item.UnitPrice);

                    itemCommand.ExecuteNonQuery();
                }

                foreach (OrderItem item in order.Items)
                {
                    string stockSql = """
                                    UPDATE Products
                                    SET Quantity = Quantity - @Quantity
                                    WHERE Id = @ProductId AND Quantity >= @Quantity;
                                    """;

                    MySqlCommand stockCommand =
                        new MySqlCommand(
                            stockSql,
                            connection,
                            transaction
                        );

                    stockCommand.Parameters.AddWithValue(
                        "@Quantity",
                        item.Quantity);

                    stockCommand.Parameters.AddWithValue(
                        "@ProductId",
                        item.ProductId);

                    int affectedRows = stockCommand.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        throw new InvalidOperationException(
                            "商品庫存更新失敗。");
                    }
                }

                //全部成功才 Commit
                transaction.Commit();

                message = "訂單建立成功。";
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();

                message = "訂單建立失敗。";
                return false;
            }
        }
    }
}