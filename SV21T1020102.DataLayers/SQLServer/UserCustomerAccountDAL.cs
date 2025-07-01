using System.Data;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
	public class UserCustomerAccountDAL : BaseDAL, IsUserCustomerAccount
	{
		public UserCustomerAccountDAL(string connecttionString) : base(connecttionString)
		{
			
		}
		
		public UserCustomerAccount? Authorzie(string name, string password)
		{
			UserCustomerAccount? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"select CustomerID as UserID, Email as UserName, CustomerName, ContactName, Email, Address, Phone
                           from Customers where Email=@Email AND Password=@Password";
				var parameters = new
				{
					Email = name,
					Password = password
				};
				data = connection.QuerySingleOrDefault<UserCustomerAccount>(sql, parameters);
				connection.Close();
			}
			return data;
		}

		public bool ChangeInfo(Customer data)
		{
			
            bool result = false;
            using (var connection = OpenConnection()) 
            {
                var sql = @"update Customers
                            Set CustomerName=@CustomerName,
                                ContactName=@ContactName,
                                Phone=@Phone,
                                Address=@Address
                             
                            Where Email=@Email";
                var parameter = new
                {
                    Email= data.Email,
                    CustomerName = data.CustomerName,
                    ContactName = data.ContactName,
                    Phone = data.Phone,
                    Address = data.Address,
                 
                };
                result = connection.Execute(sql:sql,param: parameter,commandType:CommandType.Text)>0;
                connection.Close();
                return result;
            }
		}

        public bool ChangePassword(string name, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers
                            set Password=@newPassword
                            where Email =@Email and Password=@oldPassword";
                var parameters = new
                {
                    Email = name,

                    OldPassword = oldPassword,
                    NewPassword = newPassword,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public int Register(Customer data)
        {
           int id = 0;
            using (var connection = OpenConnection()) 
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                            begin
                            insert into Customers(CustomerName,ContactName,Phone,Province,Email,Password) 
                            values(@CustomerName,@ContactName,@Phone,@Province,@Email,@Password)
                            select @@IDENTITY 
                            end";
                var parameters = new
                {
                    CustomerName= data.CustomerName,
                    ContactName= data.ContactName,
                    Phone = data.Phone,
                    Province = data.Province,
                    Email =data.Email,
                    Password = data.Password
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public bool ValidatePassword(string userName, string password)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = "SELECT Password FROM Customers WHERE Email=@Email";
                var param = new
                {
                    Email = userName
                };
                var oldpass = cn.QueryFirstOrDefault<string>(sql: sql, param: param, commandType: CommandType.Text);
                result = oldpass != null && oldpass.Equals(password);

            }
            return result;
        }


    }
}
