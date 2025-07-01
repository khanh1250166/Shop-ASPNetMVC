using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DomainModels
{
    public class UserAccount
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; }= "";   
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Roles { get; set; } = ""; 
    }
}
