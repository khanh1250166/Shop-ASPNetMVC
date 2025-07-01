using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SV21T1020102.BusineesLayers;

namespace SV21T1020102.Web.Controllers
{
	[Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password) 
        {
            var data = BusineesLayers.AccountDataService.Authorzie(username, password);
            //Kiem tra thong tin dau vao
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username)) 
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }
            // TODO: Kiểm tra thông tin đăng nhập
            if (data == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            if (username !=data.UserName ) 
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
           
          
            var userData = new WebUserData 
            {
                UserId= data.UserId,
                UserName = data.UserName,
                DisplayName = data.DisplayName,
                Photo= data.Photo,
                Roles = data.Roles.Split(',').ToList()
            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenined() 
        {
            return Content("K thanh cong ");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
		[HttpPost]
		public IActionResult ChangePassword(string email, string newPassword, string oldPassword, string confirmPassword)
		{
			// Kiểm tra nếu các trường để trống
			if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(confirmPassword))
			{
				ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin mật khẩu!");
				return View();
			}

			// Kiểm tra nếu mật khẩu mới và mật khẩu xác nhận không khớp
			if (newPassword != confirmPassword)
			{
				ModelState.AddModelError("Error", "Mật khẩu xác nhận không khớp với mật khẩu mới!");
				return View();
			}

			// Kiểm tra mật khẩu cũ có hợp lệ không
			var result = AccountDataService.ValidatePassword(email, oldPassword);
			if (!result)
			{
				ModelState.AddModelError("Error", "Mật khẩu cũ không đúng!");
				return View();
			}

			// Thực hiện thay đổi mật khẩu
			var isPasswordChanged = AccountDataService.ChangePassword(email, oldPassword, newPassword);
			if (!isPasswordChanged)
			{
				ModelState.AddModelError("Error", "Đã có lỗi xảy ra khi thay đổi mật khẩu. Vui lòng thử lại sau.");
				return View();
			}

			// Thành công
			TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
			return RedirectToAction("Login"); // Chuyển hướng đến trang hồ sơ cá nhân hoặc trang khác
		}

	}
}
