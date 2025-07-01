using System.Data;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class UserAccountDAL : BaseDAL, IsUserAccount
    {
        public UserAccountDAL(string connecttionString) : base(connecttionString)
        {
        }

        public UserAccount? Authorzie(string name, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID, Email as UserName, FullName, Email, Photo, Password, Roles
                           from Employees where Email=@Email AND Password=@Password";
                var parameters = new
                {
                    Email=name,
                    Password=password
                };
                data = connection.QuerySingleOrDefault<UserAccount>(sql, parameters);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string name, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees
                            set Password=@newPassword
                            where Email =@Email and Password=@oldPassword";
                var parameters= new
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
        public bool ValidatePassword(string userName, string password)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = "SELECT Password FROM Employees WHERE Email=@Email";
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
