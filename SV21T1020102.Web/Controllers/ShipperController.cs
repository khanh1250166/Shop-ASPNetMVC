using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;

namespace SV21T1020102.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class ShipperController : Controller
	{
        public const int PAGE_SIZE = 2;
        public const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondication";
        public IActionResult Index()
		{
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShipers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
		{
            var data = new Shipper()
            {
                ShipperID = 0,    
            };
            ViewBag.Title = "Bổ Sung Người Giao Hàng Mới";
            return View("Edit", data);
           
		}
		public IActionResult Edit(int id = 0) {
            ViewBag.Title = "Cập Nhật Thông Tin Dữ Liệu Người Giao Hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ Sung Người Giao Hàng Mới" : "Cập Nhật Thông Tin Người Giao Hàng";

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");           
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của người giao hàng");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            if (data.ShipperID == 0)
            {
                //Them
                int id = CommonDataService.AddShipper(data);
                if (id < 0) 
                { 
                    ModelState.AddModelError(nameof(data.Phone), "Sô điện thoại bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                //CapNhat
                bool result=CommonDataService.UpdateShipper(data);
                if (result == false) 
                {
                    ModelState.AddModelError(nameof(data.Phone), "Sô điện thoại bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
		public IActionResult Delete(int id = 0)
		{
            ViewBag.Title = "Xóa Người Giao Hàng";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)

                return RedirectToAction("Index");

            return View(data);
        }
	}
}
