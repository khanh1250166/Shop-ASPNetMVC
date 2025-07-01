using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;

namespace SV21T1020102.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
	{
		public const int PAGE_SIZE = 2;
        public const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondication";     
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create() {
            var data = new Category()
            {
                CategoryID = 0,
               
            };
            ViewBag.Title = "Bổ Sung Loại Hàng Mới";
            return View("Edit", data);
        }
		public IActionResult Edit(int id =0)
		{
            ViewBag.Title = "Cập Nhật Thông Tin Dữ Liệu Loại Hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            if (data.CategoryID == 0)
            {
                //Them
                int id=CommonDataService.AddCategory(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng đã tồn tại");
                    return View("Edit", data);
                }
            }
            else
            {
                //CapNhat

               bool result=CommonDataService.UpdateCategory(data);
                if (result ==false)
                {
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng đã tồn tại");
                    return View("Edit", data);
                }

            }
            return RedirectToAction("Index");
        }
		public IActionResult Delete(int id = 0) {
            ViewBag.Title = "Xóa Loại Hàng";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if (data == null)

                return RedirectToAction("Index");

            return View(data);
        }
	}
}
