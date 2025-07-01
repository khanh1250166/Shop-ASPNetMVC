using System.Collections;
using System.Data;
using System.Linq.Expressions;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>,ISimpleQueryDAL<Supplier>
    {
        public SupplierDAL(string connecttionString) : base(connecttionString)
        {
        }

        public Type ElementType => throw new NotImplementedException();

        public Expression Expression => throw new NotImplementedException();

        public IQueryProvider Provider => throw new NotImplementedException();

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                                    values(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                                    select @@IDENTITY
                                end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                   
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
            using (var conn = OpenConnection()) {
                var sql = @"Select COUNT(*)
                            From Suppliers
                            Where (SupplierName like @searchValue) or (ContactName like @searchValue)";
                var parameter = new
                {
                    searchValue
                };
            count= conn.ExecuteScalar<int>(sql:sql, param:parameter, commandType: System.Data.CommandType.Text);
            conn.Close();  
            }
                return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Suppliers where SupplierID = @SupplierID"; ;
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var conn = OpenConnection())
            {
                var sql = @"Select * From Suppliers where SupplierID=@SupplierID";
                var parameter = new
                {
                    SupplierID = id,
                };
                data = conn.QueryFirstOrDefault<Supplier>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return data;
        }

        

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @" if exists(Select * from  Products where SupplierID=@SupplierID)
	                            select 1
                            else 
	                            select 0;";
                var parameter = new
                {
                    SupplierID = id,
                };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> listtt = new List<Supplier>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection())
            {
                var sql = @"select * 
                            from (
		                            select * ,
				                            ROW_NUMBER()over (order by SupplierName) as RowNumber
		                            from Suppliers
		                            where (SupplierName like @searchValue) or (ContactName like @searchValue)
	                            )as t
                            where (@pageSize=0)
                            or	(t.RowNumber between (@page-1)*@pageSize+1 and @page *@pageSize)
                            order by RowNumber"; 
                var parameter = new
                {
                    page,
                    pageSize,
                    searchValue
                };
                listtt = conn.Query<Supplier>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
                return listtt;
            }
        }

        public List<Supplier> Listt()
        {
            List<Supplier> data = new List<Supplier>();
            using (var conn = OpenConnection())
            {
                var sql = @"select SupplierName,SupplierID from Suppliers";
                data = conn.Query<Supplier>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            };
            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"  if not exists(Select* from Suppliers where SupplierID <> @SupplierID and Email= @Email)
                                begin
                                update Suppliers 
                                set SupplierName = @SupplierName,
                                    ContactName = @ContactName,
                                    Province = @Province,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email
                                where SupplierID = @SupplierID
                                end";
                var parameters = new
                {
                    SupplierID = data.SupplierID,
                    SupplierName = data.SupplierName,
                    ContactName = data.ContactName,
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                 
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }

        
    }
}
