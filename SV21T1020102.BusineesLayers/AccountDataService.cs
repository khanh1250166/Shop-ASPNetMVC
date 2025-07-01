using SV21T1020102.DataLayers;
using SV21T1020102.DataLayers.SQLServer;
using SV21T1020102.DomainModels;

namespace SV21T1020102.BusineesLayers
{
    public class AccountDataService
    {
        private static readonly IsUserAccount accountDB;
        private static readonly IsUserCustomerAccount accountCustomerDB;
        static AccountDataService()
        {
            accountDB = new UserAccountDAL(Configuration.ConnectionString);
            accountCustomerDB = new UserCustomerAccountDAL(Configuration.ConnectionString);
        }

        public static UserAccount? Authorzie(string name, string password) 
        {
            return accountDB.Authorzie(name, password);
        }
        public static bool ChangePassword(string name, string oldPassword, string newPassword)
        {
            return accountDB.ChangePassword(name, oldPassword, newPassword);
        }
        public static bool ValidatePassword(string name, string password) 
        {
            return accountDB.ValidatePassword(name, password);
        }
        public static UserCustomerAccount? AuthorzieCustomer(string name, string password) 
        {
            return accountCustomerDB.Authorzie(name, password);
        }
        public static bool CustomerChangePassword(string name, string oldPassword, string newPassword)
        {
            return accountCustomerDB.ChangePassword(name, oldPassword, newPassword);
        }
        public static bool CustomerValidatePassword(string name, string password)
        {
            return accountCustomerDB.ValidatePassword(name, password);
        }
        public static bool CustomerChangeInfo(Customer data) 
        {
            return accountCustomerDB.ChangeInfo(data);
        }
        public static int Register(Customer data) 
        {
            return accountCustomerDB.Register(data);
        }

    }
}
