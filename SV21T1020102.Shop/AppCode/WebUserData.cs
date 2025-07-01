using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SV21T1020102.Shop
{
    /// <summary>
    /// Lưu giữ thông tin trên cookie
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
      
        /// <summary>
        /// Danh sách các claim
        /// </summary>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserId),UserId),   
                    new Claim(nameof(UserName),UserName),
                    new Claim(nameof(CustomerName),CustomerName),
                    new Claim(nameof(ContactName),ContactName),
                    new Claim(nameof(Email),Email),
                    new Claim(nameof(Phone),Phone),
                    new Claim(nameof(Address),Address),
                 
                };
                
                    return claims;                             
            }
        }
        /// <summary>
        /// Tạo ClaimsPrincipal dựa trên thông tin cuả người dùng (cần lưu trong cookie)
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        { 
            var indentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principals = new ClaimsPrincipal(indentity);
            return principals;
        }
    }
    
}
