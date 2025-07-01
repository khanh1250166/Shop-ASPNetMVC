using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class EmloyeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmloyeeDAL(string connecttionString) : base(connecttionString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate,Address,Phone,Email,Photo,IsWorking)
                                    values(@FullName, @BirthDate,@Address,@Phone, @Email,@Photo,@IsWorking);
                                    select @@IDENTITY
                                end";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate ,
                    Address=data.Address ??"",                   
                    Phone = data.Phone ?? "",                  
                    Email = data.Email ?? "",
                    Photo=data.Photo,
                    IsWorking=data.IsWorking
                   
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
		                    from Employees
		                    where (FullName like @searchValue)";
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
                var sql = @"delete from Employees where EmployeeID = @EmployeeID"; ;
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? data = null;
            using (var conn = OpenConnection())
            {
                var sql = @"Select * From Employees where EmployeeID=@EmployeeID";
                var parameter = new
                {
                    EmployeeID = id,
                };
                data = conn.QueryFirstOrDefault<Employee>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @" if exists(Select * from  Orders where EmployeeID=@EmployeeID)
	                            select 1
                            else 
	                            select 0;";
                var parameter = new
                {
                    EmployeeID = id,
                };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> list = new List<Employee>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection())
            {
                var sql = @"select * 
                            from (
		                            select * ,
				                            ROW_NUMBER()over (order by FullName) as RowNumber
		                            from Employees
		                            where (FullName like @searchValue)
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
                list = conn.Query<Employee>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return list;
        }

        public bool Update(Employee data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(Select* from Employees where EmployeeID <> @EmployeeID and Email= @Email)
                                begin
                                    update Employees 
                                    set FullName = @FullName,
                                        BirthDate = @BirthDate, 
                                        Address=@Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Photo = @Photo,
                                        IsWorking=@IsWorking
                                    where EmployeeID = @EmployeeID
                                  end";
                var parameters = new
                {
                    EmployeeID = data.EmployeeId,
                    FullName = data.FullName,
                    BirthDate=data.BirthDate,
                    Address=data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Photo = data.Photo,
                    IsWorking=data.IsWorking

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }
    }
}
