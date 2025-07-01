using System;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class OrderDAL : BaseDAL, IOrderDAL,ISimpleQueryDAL<Order>
    {
        public OrderDAL(string connecttionString) : base(connecttionString)
        {
        }

        public int Add(Order data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Orders(CustomerID, OrderTime,
                            DeliveryProvince, DeliveryAddress,
                            EmployeeID, Status)
                            values(@CustomerID, getdate(),
                            @DeliveryProvince, @DeliveryAddress,
                            @EmployeeID, @Status);
                            select @@identity";
                var param = new
                {
                    CustomerId = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    Status = data.Status,
                };
                id = connection.ExecuteScalar<int>(sql, param: param, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

		public int AddForCustomer(Order data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"insert into Orders(CustomerID, OrderTime,
                            DeliveryProvince, DeliveryAddress,
                            Status)
                            values(@CustomerID, getdate(),
                            @DeliveryProvince, @DeliveryAddress,
                            @Status);
                            select @@identity";
				var param = new
				{
					CustomerId = data.CustomerID,
					OrderTime = data.OrderTime,
					DeliveryProvince = data.DeliveryProvince,
					DeliveryAddress = data.DeliveryAddress,			
					Status = data.Status,
				};
				id = connection.ExecuteScalar<int>(sql, param: param, commandType: CommandType.Text);
				connection.Close();
			}
			return id;
		}
        public int CountNoSearch(int customerId )
        {
            int count = 0;

            using (var conn = OpenConnection())
            {
                var sql = @"select count(*)
                    from Orders as o
                    left join Customers as c on o.CustomerID = c.CustomerID
                    left join Employees as e on o.EmployeeID = e.EmployeeID
                    left join Shippers as s on o.ShipperID = s.ShipperID

                    where (o.CustomerID = @CustomerID)";  // Chỉ tìm theo CustomerID

                var parameter = new
                {
                    customerId = customerId
                };

                count = conn.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }

            return count;
        }
        public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))

                searchValue = "%" + searchValue + "%";
            using (var conn = OpenConnection())
            {
                var sql = @"select count(*)
                            from Orders as o
                            left join Customers as c on o.CustomerID = c.CustomerID
                            left join Employees as e on o.EmployeeID = e.EmployeeID
                            left join Shippers as s on o.ShipperID = s.ShipperID

                            where (@Status = 0 or o.Status = @Status)
                            and (@FromTime is null or o.OrderTime >= @FromTime)
                            and (@ToTime is null or o.OrderTime <= @ToTime)
                            and (@SearchValue = N''
                            or c.CustomerName like @SearchValue
                            or e.FullName like @SearchValue
                            or s.ShipperName like @SearchValue)";
                var parameter = new
                {
                    status = status,
                    fromTime = fromTime,
                    toTime = toTime,
                    searchValue = searchValue
                };
                count=conn.ExecuteScalar<int>(sql:sql,param:parameter,commandType:System.Data.CommandType.Text);
                conn.Close();
            }
            return count;
        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @"delete from OrderDetails where OrderID = @OrderID;
                            delete from Orders where OrderID = @OrderID";
                var parameter = new
                {
                   OrderID = orderID,
                };
                result = conn.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                conn.Close ();
            }
            return result;
        }

        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @"delete from OrderDetails
                            where OrderID = @OrderID and ProductID = @ProductID";

                var parameter = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                };
                result = conn.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
                conn.Close();
            }
            return result;
        }

        public Order? Get(int orderID)
        {
            Order? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select o.*,
                            c.CustomerName,

                            c.ContactName as CustomerContactName,
                            c.Address as CustomerAddress,
                            c.Phone as CustomerPhone,
                            c.Email as CustomerEmail,
                            e.FullName as EmployeeName,
                            s.ShipperName,
                            s.Phone as ShipperPhone

                            from Orders as o
                            left join Customers as c on o.CustomerID = c.CustomerID
                            left join Employees as e on o.EmployeeID = e.EmployeeID
                            left join Shippers as s on o.ShipperID = s.ShipperID

                            where o.OrderID = @OrderID";
                var parameter = new
                {
                    OrderID = orderID,
                };
                data=connection.QueryFirstOrDefault<Order>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<Order>? GetbyCustomer(int CustomerID, int page=1,int pageSize=0)
        {
            List<Order> list = new List<Order>();
            using (var connection = OpenConnection())
            {
                var sql = @"Select o.*,
                                   c.CustomerName,
                                   c.ContactName as CustomerContactName,
                                   c.Address as CustomerAddress,
                                   c.Phone as CustomerPhone,
                                   c.Email as CustomerEmail,
                                   e.FullName as EmployeeName,
                                   s.ShipperName,
                                   s.Phone as ShipperPhone
                            from Orders as o
                            left join Customers as c on o.CustomerID = c.CustomerID
                            left join Employees as e on o.EmployeeID = e.EmployeeID
                            left join Shippers as s on o.ShipperID = s.ShipperID
                            where o.CustomerID = @CustomerID
                            order by o.OrderTime desc
                            offset (@Page - 1) * @PageSize rows fetch next @PageSize rows only;";
                var parameter = new
                {
                    CustomerID = CustomerID,
                    Page=page,
                    PageSize = pageSize
                };
                list = connection.Query<Order>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                return list;
            }
        }

        public OrderDetail? GetDetail(int orderID, int productID)
        {
            OrderDetail? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails as od
                            join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID and od.ProductID = @ProductID";
                var parameter = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                };
                data=connection.QueryFirstOrDefault<OrderDetail>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<Order> List(int page = 1, int pageSize = 0, string searchValue = "",  int status = 0, DateTime? fromTime = null, DateTime? toTime = null)
        {
            List<Order> list = new List<Order>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
                            select row_number() over(order by o.OrderTime desc) as RowNumber,
                            o.*,

                            c.CustomerName,
                            c.ContactName as CustomerContactName,
                            c.Address as CustomerAddress,
                            c.Phone as CustomerPhone,
                            c.Email as CustomerEmail,
                            e.FullName as EmployeeName,
                            s.ShipperName,
                            s.Phone as ShipperPhone

                            from Orders as o
                            left join Customers as c on o.CustomerID = c.CustomerID
                            left join Employees as e on o.EmployeeID = e.EmployeeID
                            left join Shippers as s on o.ShipperID = s.ShipperID

                            where (@Status = 0 or o.Status = @Status)
                            and (@FromTime is null or o.OrderTime >= @FromTime)
                            and (@ToTime is null or o.OrderTime <= @ToTime)
                            and (@SearchValue = N''
                            or c.CustomerName like @SearchValue
                            or e.FullName like @SearchValue
                            or s.ShipperName like @SearchValue)

                            )
                            select * from cte
                            where (@PageSize = 0)

                            or (RowNumber between (@Page - 1) * @PageSize + 1 and @Page * @PageSize)

                            order by RowNumber";
                var parameter = new
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    FromTime = fromTime,
                    ToTime = toTime,
                    SearchValue = searchValue
                };
                list= connection.Query<Order>(sql:sql,param:parameter,commandType: System.Data.CommandType.Text).ToList();
                connection.Close ();
            }
            return list;
        }

        public IList<OrderDetail> ListDetails(int orderID)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            using (var connection = OpenConnection())
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails as od
                            join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID";
                var parameter = new
                {
                    OrderID = orderID,
                };
                list = connection.Query<OrderDetail>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        public List<Order> Listt()
        {
            List<Order> data = new List<Order>();
            using (var conn = OpenConnection())
            {
                var sql = @"select * from OrderStatus";
                data = conn.Query<Order>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            };
            return data;
        }

        public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if exists(select * from OrderDetails
                                      where OrderID = @OrderID and ProductID = @ProductID)
                            begin
                                update OrderDetails
                                set Quantity = @Quantity,
                                    SalePrice = @SalePrice
                                where OrderID = @OrderID and ProductID = @ProductID
                            end
                            else
                            begin
                                insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                                values(@OrderID, @ProductID, @Quantity, @SalePrice)

                                update Products
                                set CurrentStock = CurrentStock - @Quantity
                                where ProductID = @ProductID
                            end";
                var parameter = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                    Quantity = quantity,
                    SalePrice = salePrice
                };
                result= connection.Execute(sql:sql, param:parameter, commandType:System.Data.CommandType.Text)>0;
            }
            return result;
        }

        public bool Update(Order data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {            
                if (data.Status == Constants.ORDER_CANCEL)
                {
                    // Lấy danh sách mặt hàng trong đơn
                    var orderDetails = connection.Query<OrderDetail>(
                        "SELECT ProductID, Quantity FROM OrderDetails WHERE OrderID = @OrderID",
                        new { OrderID = data.OrderID });

                    foreach (var item in orderDetails)
                    {
                        // Cộng lại số lượng vào kho
                        connection.Execute(
                            "UPDATE Products SET CurrentStock = CurrentStock + @Quantity WHERE ProductID = @ProductID",
                            new { item.ProductID, item.Quantity });

                        // Ghi lịch sử hoàn kho
                        connection.Execute(
                            @"INSERT INTO InventoryTransactions(ProductID, TransactionType, QuantityChange, TransactionDate, Note, OrderID)
                      VALUES(@ProductID, 'Return', @Quantity, GETDATE(), N'Hoàn kho khi huỷ đơn hàng', @OrderID)",
                            new
                            {
                                ProductID = item.ProductID,
                                Quantity = item.Quantity,
                                OrderID = data.OrderID
                            });
                    }
                }

                // Cập nhật đơn hàng
                var sql = @"UPDATE Orders
                    SET CustomerID = @CustomerID,
                        OrderTime = @OrderTime,
                        DeliveryProvince = @DeliveryProvince,
                        DeliveryAddress = @DeliveryAddress,
                        EmployeeID = @EmployeeID,
                        AcceptTime = @AcceptTime,
                        ShipperID = @ShipperID,
                        ShippedTime = @ShippedTime,
                        FinishedTime = @FinishedTime,
                        Status = @Status
                    WHERE OrderID = @OrderID";

                var parameter = new
                {
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                    OrderID = data.OrderID,
                };

                result = connection.Execute(sql, parameter) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
