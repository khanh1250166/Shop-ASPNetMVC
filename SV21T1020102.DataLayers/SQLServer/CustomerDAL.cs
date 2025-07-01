using System.Data;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>,ISimpleQueryDAL<Customer>
    {
        public CustomerDAL(string connecttionString) : base(connecttionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                    values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                                    select @@IDENTITY
                                end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
           int count = 0;
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection()) 
            {
                var sql = @"select COUNT(*)
		                    from Customers
		                    where (CustomerName like @searchValue)or (ContactName like @searchValue)";
                var parameter = new
                {
                    searchValue
                };
                count = conn.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return count;

        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerId = @CustomerId"; ;
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var conn=OpenConnection()) 
            {
                var sql = @"Select * From Customers where CustomerID=@CustomerId";
                var parameter = new
                {
                    CustomerId = id,
                };
                data = conn.QueryFirstOrDefault<Customer>(sql: sql, param: parameter,commandType:System.Data.CommandType.Text);
                conn.Close();
            }
                return data;
        }

        public bool InUsed(int id)
        {
           bool result= false;
            using (var conn = OpenConnection())
            {
                var sql = @" if exists(Select * from  Orders where CustomerID=@CustomerId)
	                            select 1
                            else 
	                            select 0;";
                var parameter = new
                {
                    CustomerId = id,
                };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close() ;
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection()) 
            {
                var sql = @"select * 
                            from (
		                            select * ,
				                            ROW_NUMBER()over (order by CustomerName) as RowNumber
		                            from Customers
		                            where (CustomerName like @searchValue) or (ContactName like @searchValue)
	                            )as t
                            where (@pageSize=0)
                            or	(t.RowNumber between (@page-1)*@pageSize+1 and @page *@pageSize)
                            order by RowNumber";
                var parameter = new 
                { 
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = conn.Query<Customer>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
                return data;
        }

        public List<Customer> Listt()
        {
            List<Customer> data = new List<Customer>();
            using (var conn = OpenConnection())
            {
                var sql = @"select CustomerName,CustomerID from Customers";
                data = conn.Query<Customer>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            };
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(Select* from Customers where CustomerID <> @CustomerId and Email= @Email)
                                begin
                            update Customers 
                                set CustomerName = @CustomerName,
                                    ContactName = @ContactName,
                                    Province = @Province,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    IsLocked = @IsLocked
                                where CustomerId = @CustomerId
                                end";
                var parameters = new
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.CustomerName,
                    ContactName =data.ContactName,
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    IsLocked = data.IsLocked,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }
    }
}
