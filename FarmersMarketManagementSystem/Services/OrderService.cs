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
                                    (CustomerId, OrderDate, TotalPrice, Status)
                                VALUES
                                    (@CustomerId, @OrderDate, @TotalPrice, @Status);
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

                orderCommand.Parameters.AddWithValue(
                    "@Status",
                    (int)order.Status);

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
        public Order? GetOrderById(int id)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, CustomerId, OrderDate, TotalPrice, Status, MergedIntoOrderId
                        FROM Orders
                        WHERE Id = @Id;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                Order order = new Order();

                order.Id = reader.GetInt32("Id");
                order.CustomerId = reader.GetInt32("CustomerId");
                order.OrderDate = reader.GetDateTime("OrderDate");
                order.TotalPrice = reader.GetDecimal("TotalPrice");
                order.Status = (OrderStatus)reader.GetInt32("Status");
                if (!reader.IsDBNull(reader.GetOrdinal("MergedIntoOrderId")))
                {
                    order.MergedIntoOrderId = reader.GetInt32("MergedIntoOrderId");
                }
                else
                {
                    order.MergedIntoOrderId = null;
                }

                return order;
            }

            return null;
        }
        public OrderDetail? GetOrderDetail(int orderId)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT
                            o.Id AS OrderId,
                            c.FirstName,
                            c.LastName,
                            o.OrderDate,
                            o.TotalPrice,
                            p.Id AS ProductId,
                            p.ProductName,
                            oi.Quantity,
                            oi.UnitPrice,
                            o.Status,
                            o.MergedIntoOrderId
                        FROM Orders o
                        JOIN Customers c
                            ON o.CustomerId = c.Id
                        JOIN OrderItems oi
                            ON o.Id = oi.OrderId
                        JOIN Products p
                            ON oi.ProductId = p.Id
                        WHERE o.Id = @OrderId;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@OrderId", orderId);

            using MySqlDataReader reader =
                command.ExecuteReader();

            OrderDetail? detail = null;

            while (reader.Read())
            {
                if (detail == null)
                {
                    detail = new OrderDetail();

                    detail.OrderId = reader.GetInt32("OrderId");
                    detail.CustomerName =
                        $"{reader.GetString("FirstName")} {reader.GetString("LastName")}";

                    detail.OrderDate = reader.GetDateTime("OrderDate");
                    detail.TotalPrice = reader.GetDecimal("TotalPrice");
                    detail.Status = (OrderStatus)reader.GetInt32("Status");
                    detail.MergedIntoOrderId = reader.IsDBNull(reader.GetOrdinal("MergedIntoOrderId")) 
                        ? null 
                        : reader.GetInt32("MergedIntoOrderId");
                }

                detail.Items.Add(new OrderDetailItem
                {
                    ProductId = reader.GetInt32("ProductId"),
                    ProductName = reader.GetString("ProductName"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice")
                });
            }

            return detail;
        }
        public List<Order> GetOrdersByCustomer(int customerId)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, CustomerId, OrderDate, TotalPrice, Status, MergedIntoOrderId
                        FROM Orders
                        WHERE CustomerId = @CustomerId
                        ORDER BY OrderDate DESC;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@CustomerId", customerId);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<Order> orders = new List<Order>();

            while (reader.Read())
            {
                Order order = new Order();

                order.Id = reader.GetInt32("Id");
                order.CustomerId = reader.GetInt32("CustomerId");
                order.OrderDate = reader.GetDateTime("OrderDate");
                order.TotalPrice = reader.GetDecimal("TotalPrice");
                order.Status = (OrderStatus)reader.GetInt32("Status");
                order.MergedIntoOrderId = reader.IsDBNull(reader.GetOrdinal("MergedIntoOrderId"))
                        ? null
                        : reader.GetInt32("MergedIntoOrderId");

                orders.Add(order);
            }

            return orders;
        }
        public bool UpdateOrderStatus(
            int orderId,
            OrderStatus newStatus,
            out string message)
        {
            Order? order = GetOrderById(orderId);

            if (order == null)
            {
                message = "找不到此訂單。";
                return false;
            }

            if (order.Status == OrderStatus.Completed ||
                order.Status == OrderStatus.Cancelled ||
                order.Status == OrderStatus.Merged)
            {
                message = $"目前訂單狀態為{order.Status}，不允許更新此訂單狀態。";
                return false;
            }

            if (order.Status == OrderStatus.Pending)
            {
                if (newStatus != OrderStatus.Paid &&
                    newStatus != OrderStatus.Cancelled)
                {
                    message = "Pending 只能變更為 Paid 或 Cancelled。";
                    return false;
                }
            }

            if (order.Status == OrderStatus.Paid)
            {
                if (newStatus != OrderStatus.Completed &&
                    newStatus != OrderStatus.Cancelled)
                {
                    message = "Paid 只能變更為 Completed 或 Cancelled。";
                    return false;
                }
            }

            if (newStatus == OrderStatus.Cancelled)
            {
                bool cancelResult = CancelOrder(orderId, out string cancelMessage);
                message = cancelMessage;
                return cancelResult;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        UPDATE Orders
                        SET Status = @Status
                        WHERE Id = @OrderId;
                        """;
            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Status", (int)newStatus);
            command.Parameters.AddWithValue("@OrderId", orderId);

            int affectedRows = command.ExecuteNonQuery();


            if (affectedRows > 0)
            {
                message = "訂單狀態更新成功。";
                return true;
            }
            else
            {
                message = "訂單狀態更新失敗。";
                return false;
            }

        }
        public bool CancelOrder(int orderId, out string message)
        {
            Order? order = GetOrderById(orderId);

            if (order == null)
            {
                message = "找不到此訂單。";
                return false;
            }

            if (order.Status == OrderStatus.Completed ||
                order.Status == OrderStatus.Cancelled ||
                order.Status == OrderStatus.Merged)
            {
                message =
                    $"目前訂單狀態為 {order.Status}，無法取消訂單。";

                return false;
            }

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();
            try
            {
                string sql = """
                            SELECT ProductId, Quantity
                            FROM OrderItems
                            WHERE OrderId = @OrderId;
                            """;

                MySqlCommand command =
                    new MySqlCommand(
                        sql,
                        connection,
                        transaction);

                command.Parameters.AddWithValue(
                    "@OrderId",
                    orderId);

                List<OrderItem> orderItems =
                    new List<OrderItem>();

                using (MySqlDataReader reader =
                    command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OrderItem orderItem =
                            new OrderItem();

                        orderItem.ProductId =
                            reader.GetInt32("ProductId");

                        orderItem.Quantity =
                            reader.GetInt32("Quantity");

                        orderItems.Add(orderItem);
                    }
                }

                foreach (OrderItem item in orderItems)
                {
                    string stockSql = """
                                    UPDATE Products
                                    SET Quantity = Quantity + @Quantity
                                    WHERE Id = @ProductId;
                                    """;

                    MySqlCommand stockCommand =
                        new MySqlCommand(
                            stockSql,
                            connection,
                            transaction);

                    stockCommand.Parameters.AddWithValue(
                        "@Quantity",
                        item.Quantity);

                    stockCommand.Parameters.AddWithValue(
                        "@ProductId",
                        item.ProductId);

                    int stockAffectedRows =
                        stockCommand.ExecuteNonQuery();

                    if (stockAffectedRows == 0)
                    {
                        throw new InvalidOperationException(
                            "商品庫存恢復失敗。");
                    }
                }

                string statusSql = """
                                UPDATE Orders
                                SET Status = @Status
                                WHERE Id = @OrderId;
                                """;

                MySqlCommand statusCommand =
                    new MySqlCommand(
                        statusSql,
                        connection,
                        transaction);

                statusCommand.Parameters.AddWithValue(
                    "@Status",
                    (int)OrderStatus.Cancelled);

                statusCommand.Parameters.AddWithValue(
                    "@OrderId",
                    orderId);

                int statusAffectedRows =
                    statusCommand.ExecuteNonQuery();

                if (statusAffectedRows == 0)
                {
                    throw new InvalidOperationException(
                        "訂單狀態更新失敗。");
                }

                transaction.Commit();

                message = "訂單取消成功。";
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();

                message = "訂單取消失敗。";
                return false;
            }
        }
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            string sql = """
                        SELECT Id, OrderId, ProductId, Quantity, UnitPrice
                        FROM OrderItems
                        WHERE OrderId = @OrderId;
                        """;

            MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@OrderId", orderId);

            using MySqlDataReader reader =
                command.ExecuteReader();

            List<OrderItem> orderItems = new List<OrderItem>();

            while (reader.Read())
            {
                OrderItem item = new OrderItem();

                item.Id = reader.GetInt32("Id");
                item.OrderId = reader.GetInt32("OrderId");
                item.ProductId = reader.GetInt32("ProductId");
                item.Quantity = reader.GetInt32("Quantity");
                item.UnitPrice = reader.GetDecimal("UnitPrice");

                orderItems.Add(item);
            }

            return orderItems;
        }
        public bool MergeOrders(
            int orderId1,
            int orderId2,
            out string message)
        {
            // 1. 取得兩張訂單
            Order? order1 = GetOrderById(orderId1);
            Order? order2 = GetOrderById(orderId2);

            if (order1 == null || order2 == null)
            {
                message = "找不到其中一張訂單。";
                return false;
            }

            // 2. 不能把同一張訂單跟自己合併
            if (order1.Id == order2.Id)
            {
                message = "不能合併同一張訂單。";
                return false;
            }

            // 3. 必須是同一位 Customer
            if (order1.CustomerId != order2.CustomerId)
            {
                message = "不同客戶的訂單無法合併。";
                return false;
            }

            // 4. Completed / Cancelled / Merged 都不能合併
            if (order1.Status == OrderStatus.Completed ||
                order1.Status == OrderStatus.Cancelled ||
                order1.Status == OrderStatus.Merged ||
                order2.Status == OrderStatus.Completed ||
                order2.Status == OrderStatus.Cancelled ||
                order2.Status == OrderStatus.Merged)
            {
                message = "已完成、已取消或已合併的訂單無法再次合併。";
                return false;
            }

            // 5. 兩張訂單狀態必須相同
            if (order1.Status != order2.Status)
            {
                message = "訂單狀態不同，無法合併。";
                return false;
            }

            // 6. 日期較新的訂單保留
            Order newOrder;
            Order oldOrder;

            if (order1.OrderDate >= order2.OrderDate)
            {
                newOrder = order1;
                oldOrder = order2;
            }
            else
            {
                newOrder = order2;
                oldOrder = order1;
            }

            // 7. 取得兩張訂單的商品
            List<OrderItem> newItems =
                GetOrderItemsByOrderId(newOrder.Id);

            List<OrderItem> oldItems =
                GetOrderItemsByOrderId(oldOrder.Id);

            using MySqlConnection connection =
                databaseConnection.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                // 8. 把舊訂單的商品合併到新訂單
                foreach (OrderItem oldItem in oldItems)
                {
                    OrderItem? existingItem =
                        newItems.FirstOrDefault(
                            item =>
                                item.ProductId == oldItem.ProductId &&
                                item.UnitPrice == oldItem.UnitPrice
                        );

                    if (existingItem != null)
                    {
                        string updateItemSql = """
                    UPDATE OrderItems
                    SET Quantity = Quantity + @Quantity
                    WHERE OrderId = @NewOrderId
                      AND ProductId = @ProductId
                      AND UnitPrice = @UnitPrice;
                    """;

                        MySqlCommand updateItemCommand =
                            new MySqlCommand(
                                updateItemSql,
                                connection,
                                transaction);

                        updateItemCommand.Parameters.AddWithValue(
                            "@Quantity",
                            oldItem.Quantity);

                        updateItemCommand.Parameters.AddWithValue(
                            "@NewOrderId",
                            newOrder.Id);

                        updateItemCommand.Parameters.AddWithValue(
                            "@ProductId",
                            oldItem.ProductId);

                        updateItemCommand.Parameters.AddWithValue(
                            "@UnitPrice",
                            oldItem.UnitPrice);

                        int affectedRows =
                            updateItemCommand.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            throw new InvalidOperationException(
                                "合併商品數量失敗。");
                        }
                    }
                    else
                    {
                        string insertItemSql = """
                    INSERT INTO OrderItems
                        (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES
                        (@NewOrderId, @ProductId, @Quantity, @UnitPrice);
                    """;

                        MySqlCommand insertItemCommand =
                            new MySqlCommand(
                                insertItemSql,
                                connection,
                                transaction);

                        insertItemCommand.Parameters.AddWithValue(
                            "@NewOrderId",
                            newOrder.Id);

                        insertItemCommand.Parameters.AddWithValue(
                            "@ProductId",
                            oldItem.ProductId);

                        insertItemCommand.Parameters.AddWithValue(
                            "@Quantity",
                            oldItem.Quantity);

                        insertItemCommand.Parameters.AddWithValue(
                            "@UnitPrice",
                            oldItem.UnitPrice);

                        int affectedRows =
                            insertItemCommand.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            throw new InvalidOperationException(
                                "新增合併商品失敗。");
                        }
                    }
                }

                // 9. 新訂單總價 = 原本兩張訂單總價相加
                decimal mergedTotalPrice =
                    newOrder.TotalPrice + oldOrder.TotalPrice;

                string updateNewOrderSql = """
                                        UPDATE Orders
                                        SET TotalPrice = @TotalPrice
                                        WHERE Id = @NewOrderId;
                                        """;

                MySqlCommand updateNewOrderCommand =
                    new MySqlCommand(
                        updateNewOrderSql,
                        connection,
                        transaction);

                updateNewOrderCommand.Parameters.AddWithValue(
                    "@TotalPrice",
                    mergedTotalPrice);

                updateNewOrderCommand.Parameters.AddWithValue(
                    "@NewOrderId",
                    newOrder.Id);

                int newOrderAffectedRows =
                    updateNewOrderCommand.ExecuteNonQuery();

                if (newOrderAffectedRows == 0)
                {
                    throw new InvalidOperationException(
                        "更新合併訂單總金額失敗。");
                }

                // 10. 舊訂單標記成 Merged，並記錄被合併到哪張訂單
                string updateOldOrderSql = """
                                            UPDATE Orders
                                            SET Status = @Status,
                                                MergedIntoOrderId = @MergedIntoOrderId
                                            WHERE Id = @OldOrderId;
                                            """;

                MySqlCommand updateOldOrderCommand =
                    new MySqlCommand(
                        updateOldOrderSql,
                        connection,
                        transaction);

                updateOldOrderCommand.Parameters.AddWithValue(
                    "@Status",
                    (int)OrderStatus.Merged);

                updateOldOrderCommand.Parameters.AddWithValue(
                    "@MergedIntoOrderId",
                    newOrder.Id);

                updateOldOrderCommand.Parameters.AddWithValue(
                    "@OldOrderId",
                    oldOrder.Id);

                int oldOrderAffectedRows =
                    updateOldOrderCommand.ExecuteNonQuery();

                if (oldOrderAffectedRows == 0)
                {
                    throw new InvalidOperationException(
                        "更新舊訂單狀態失敗。");
                }

                transaction.Commit();

                message =
                    $"訂單合併成功。訂單 #{oldOrder.Id} 已合併至訂單 #{newOrder.Id}。";

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();

                message = "訂單合併失敗。";
                return false;
            }
        }
    }
}