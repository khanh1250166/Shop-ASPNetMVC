using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers
{
	public interface IsUserCustomerAccount
	{
		UserCustomerAccount? Authorzie(string name, string password);
		bool ChangePassword(string name, string password, string newPassword);
		public bool ValidatePassword(string userName, string password);
		bool ChangeInfo(Customer data);
        int Register(Customer data);
	}
}
