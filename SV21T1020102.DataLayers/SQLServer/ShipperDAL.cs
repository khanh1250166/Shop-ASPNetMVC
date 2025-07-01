using System.Data;
using Dapper;
using SV21T1020102.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020102.DataLayers.SQLServer
{
     public class ShipperDAL : BaseDAL, ICommonDAL<Shipper> 
    {

        public ShipperDAL(string connecttionString) : base(connecttionString)
        {
        }

        public int Add(Shipper data)
        {

            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Shippers where Phone = @Phone)
                                select -1
                            else    
                                begin
                                    insert into Shippers(ShipperName, Phone)
                                    values(@ShipperName, @Phone);
                                    select @@IDENTITY 
                                end";
                var parameters = new
                {
                    ShipperName=data.ShipperName,
                    Phone=data.Phone
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
            using (var conn = OpenConnection()){
                var sql= @"select COUNT(*)
		                    from Shippers
		                    where (ShipperName like @searchValue) ";
                var parameter = new {
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
                var sql = @"delete from Shippers where ShipperID = @ShipperID"; ;
                var parameters = new
                {
                    ShipperID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var conn = OpenConnection())
            {
                var sql = @"Select * From Shippers where ShipperID=@ShipperID";
                var parameter = new
                {
                    ShipperID = id,
                };
                data = conn.QueryFirstOrDefault<Shipper>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @" if exists(Select * from  Orders where ShipperID=@ShipperID)
	                            select 1
                            else 
	                            select 0;";
                var parameter = new
                {
                    ShipperID = id,
                };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
           List<Shipper> list = new List<Shipper>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection()) {
                var sql = @"select * 
                            from (
		                            select * ,
				                            ROW_NUMBER()over (order by ShipperName) as RowNumber
		                            from Shippers
		                            where (ShipperName like @searchValue)
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
                list = conn.Query<Shipper>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
           return list;
        }


        


        public bool Update(Shipper data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(Select* from Shippers where ShipperID <> @ShipperID and Phone= @Phone)
                                begin
                                    update Shippers 
                                    set ShipperName = @ShipperName,                               
                                        Phone = @Phone  
                                    where ShipperID = @ShipperID
                                end";
                var parameters = new
                {
                    ShipperID = data.ShipperID, 
                    ShipperName = data.ShipperName,
                    Phone = data.Phone,   
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }
    }
}

