using System.Security.Claims;

namespace SV21T1020102.Shop
{
    /// <summary>
    /// Tạo thêm phương thức mở rộng cho các Pricipal để lấy thông tin của người dùng trên cookie 
    /// </summary>
    public static class WebUserExtentions
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal) 
        {
            try 
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;
                var userData = new WebUserData();
                userData.UserId = principal.FindFirstValue(nameof(userData.UserId)) ?? "";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.CustomerName = principal.FindFirstValue(nameof(userData.CustomerName)) ?? "";
                userData.ContactName = principal.FindFirstValue(nameof(userData.ContactName)) ?? "";
                userData.Email = principal.FindFirstValue(nameof(userData.Email)) ?? "";
                userData.Phone = principal.FindFirstValue(nameof(userData.Phone)) ?? "";
                userData.Address = principal.FindFirstValue(nameof(userData.Address)) ?? "";
           
                return userData;
            }
            catch 
            { 
                return null;
            }

        }
    }
}
