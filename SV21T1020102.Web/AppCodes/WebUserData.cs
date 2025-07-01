using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SV21T1020102.Web
{
    /// <summary>
    /// Lưu giữ thông tin trên cookie
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }
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
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName),
                    new Claim(nameof(Photo), Photo),
                };
                if (Roles != null)               
                    foreach (var role in Roles)                    
                        claims.Add(new Claim(ClaimTypes.Role, role));
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
