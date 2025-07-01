using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;

namespace SV21T1020102.Web.Controllers
{
	[Authorize(Roles ="admin")]
	public class CustomerController : Controller
	{
		public const int PAGE_SIZE = 30;
		public const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondication";
		public IActionResult Index()
		{
			PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
			if (condition == null)
				condition = new PaginationSearchInput()
				{
					Page = 1,
					PageSize = PAGE_SIZE,
					SearchValue = ""
				};
			return View(condition);
		}
		public IActionResult Search(PaginationSearchInput condition) 
		{
			int rowCount;
			var data = CommonDataService.ListOfCustomers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
			CustomerSearchResult model = new CustomerSearchResult()
			{
				Page = condition.Page,
				PageSize = condition.PageSize,
				SearchValue = condition.SearchValue ?? "",
				RowCount = rowCount,
				Data = data
			};
			ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
			return View(model);
		}
		public IActionResult Create() {
			var data = new Customer()
			{
                CustomerID = 0,
				IsLocked = false,
			};
			ViewBag.Title = "Bổ Sung Khách Hàng Mới";
			return View("Edit",data);
		}
		public IActionResult Edit(int id = 0) {
			ViewBag.Title = "Cập Nhật Thông Tin Dữ Liệu Khách Hàng";
			var data=CommonDataService.GetCustomer(id);
			if (data == null) 
				return RedirectToAction("Index");		
			return View(data);
		}
		[HttpPost]
		public IActionResult Save(Customer data)
		{
			ViewBag.Title = data.CustomerID == 0 ? "Bổ Sung Khách Hàng Mới" : "Cập Nhật Thông Tin Khách Hàng";

			if (string.IsNullOrWhiteSpace(data.CustomerName))
				ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
			if (string.IsNullOrWhiteSpace(data.ContactName))
				ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
			if (string.IsNullOrWhiteSpace(data.Phone))
				ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
			if (string.IsNullOrWhiteSpace(data.Email))
				ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
			if (string.IsNullOrWhiteSpace(data.Address))
				ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
			if (string.IsNullOrWhiteSpace(data.Province))
				ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh thành của khách hàng");
			if (ModelState.IsValid == false)
			{
				return View("Edit", data);
			}

			
				if (data.CustomerID == 0)
				{
					//Them
					int id = CommonDataService.AddCustomer(data);
					if (id <= 0)
					{
						ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
						return View("Edit", data);
					}
				}
				else
				{
					//CapNhat
					bool result = CommonDataService.UpdateCustomer(data);
					if (result == false)
					{
						ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
						return View("Edit", data);
					}
				}
				return RedirectToAction("Index");
			
			
		}
		public IActionResult Delete(int id= 0) {
            ViewBag.Title = "Xóa Khách Hàng";
            if (Request.Method == "POST") {
				CommonDataService.DeleteCustomer(id);
				return RedirectToAction("Index");
			}
			var data = CommonDataService.GetCustomer(id);
			if (data == null)
			
				return RedirectToAction("Index");
			
			return View(data);
		}
	}
}
