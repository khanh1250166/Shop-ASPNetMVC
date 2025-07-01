using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Shop.Models;

namespace SV21T1020102.Shop.Controllers
{
    public class AccountController : Controller
    {
        public const string ORDER_CONDITATION = "ORDER_CONDITATION";

        public const int PAGE_SIZE= 6;
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }
            var data = BusineesLayers.AccountDataService.AuthorzieCustomer(username, password);
            //Kiem tra thong tin dau vao
            
            // TODO: Kiểm tra thông tin đăng nhập
            if (data == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            if (username != data.UserName)
            {
                ModelState.AddModelError("Error", "Sai tài khoản hoặc mật khẩu");
                return View();
            }
            var userData = new WebUserData
            {
                UserId = data.UserId,
                UserName = data.UserName,
                CustomerName = data.CustomerName,
                ContactName = data.ContactName,
                Email = data.Email,
                Phone = data.Phone,
                Address = data.Address,
            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Product");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
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
			var result = AccountDataService.CustomerValidatePassword(email, oldPassword);
			if (!result)
			{
				ModelState.AddModelError("Error", "Mật khẩu cũ không đúng!");
				return View();
			}

			// Thực hiện thay đổi mật khẩu
			var isPasswordChanged = AccountDataService.CustomerChangePassword(email, oldPassword, newPassword);
			if (!isPasswordChanged)
			{
				ModelState.AddModelError("Error", "Đã có lỗi xảy ra khi thay đổi mật khẩu. Vui lòng thử lại sau.");
				return View();
			}

			// Thành công
			TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
			return RedirectToAction("Login"); 
		}
		public IActionResult Info()
        {
            PaginationInput condition = ApplicationContext.GetSessionData<PaginationInput>(ORDER_CONDITATION);
            if (condition == null)
                condition = new PaginationInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                };
            return View(condition);
        }
        public IActionResult Search( PaginationInput condition) {
            int rowCount;
            var userData = User.GetUserData();        
            var CustomerID = int.Parse(userData.UserId);

            var data = OrderDataService.ListOrderByCustomerID(out rowCount, CustomerID,condition.Page, condition.PageSize);
            OrderResult model = new OrderResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,    
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_CONDITATION, condition);
            return View(model);
        }
        [HttpGet]
        public IActionResult ChangeInfo()
        {       
            return View();
        }
        [HttpPost]
        public IActionResult ChangeInfo(Customer data)
        {
           
            if (string.IsNullOrWhiteSpace(data.CustomerName) )
            {
                ModelState.AddModelError(nameof(data.CustomerName), "Vui lòng k bỏ trống trường này"); 
           
            }
            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Vui lòng k bỏ trống trường này");
            
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng k bỏ trống trường này");
            
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Vui lòng k bỏ trống trường này");
           
            }
          
            if (ModelState.IsValid == false)
            {
                return View(data);
            }
            if (!ModelState.IsValid)
            {
                return View(data);
            }
            var result = AccountDataService.CustomerChangeInfo(data);
            if (!result)
            {
                ModelState.AddModelError("Erorr", "Không thể cập nhật thông tin.");
                return View(data);
            }

            
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Resign() 
        {
            var Data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View(Data);
        }
        [HttpPost]
        public IActionResult Resign(Customer data)
        {
            // Kiểm tra các trường nhập liệu
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            else if (!Regex.IsMatch(data.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                ModelState.AddModelError(nameof(data.Email), "Email không hợp lệ");

            if (string.IsNullOrWhiteSpace(data.Password))
                ModelState.AddModelError(nameof(data.Password), "Vui lòng nhập mật khẩu của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành của khách hàng");

            // Kiểm tra xác nhận mật khẩu
            if (string.IsNullOrWhiteSpace(data.ConfirmPassword))
                ModelState.AddModelError(nameof(data.ConfirmPassword), "Vui lòng xác nhận mật khẩu");
            else if (data.Password != data.ConfirmPassword)
                ModelState.AddModelError(nameof(data.ConfirmPassword), "Mật khẩu và xác nhận mật khẩu không khớp");

            // Nếu có lỗi, trả lại View để hiển thị các thông báo lỗi
            if (!ModelState.IsValid)
                return View(data);

            // Đăng ký người dùng
            int id = AccountDataService.Register(data);
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(data.Email), "Email đã bị trùng. Vui lòng thử lại với email khác.");
                return View(data);  // Trả về lại trang đăng ký
            }

            // Nếu đăng ký thành công, chuyển hướng đến trang đăng nhập
            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login", "Account");
        }

    }
}
