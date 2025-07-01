using System.Buffers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;

namespace SV21T1020102.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class EmployeeController : Controller
	{
        public const int PAGE_SIZE = 6;
        public const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondication";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create() {
            var data = new Employee()
            {
                EmployeeId = 0,
                IsWorking = true,
                Photo="nophoto.png"
            };
            ViewBag.Title = "Bổ Sung Nhân Viên Mới";
            return View("Edit", data);
        }
        public IActionResult Edit(int id =0)
        {
            ViewBag.Title = "Cập Nhật Thông Tin Dữ Liệu Nhân Viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Employee data,string _BirthDate,IFormFile? _Photo)
        {
            ViewBag.Title = data.EmployeeId == 0 ? "Bổ Sung Nhân Viên Mới" : "Cập Nhật Thông Tin Khách Hàng";

            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên khách hàng không được để trống");           
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập địa chỉ của khách hàng");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            // Xu ly ngay sinh
            DateTime? d = _BirthDate.ToDateTime();
            if (d.HasValue)
                data.BirthDate = d.Value;
            //Xu ly anh
            if (_Photo != null) 
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\Employee",fileName);
                using (var stream = new FileStream(filePath,FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo=fileName;
            }


            if (data.EmployeeId == 0)
            {
                //Them
                int id =CommonDataService.AddEmployee(data);
                if (id < 0) 
                {
                    ModelState.AddModelError(nameof(data.Email), "Email của khách hàng bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                //CapNhat
                bool result=CommonDataService.UpdateEmployee(data);
                if(result == false)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email của khách hàng bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            ViewBag.Title = "Xóa Nhân Viên";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)

                return RedirectToAction("Index");

            return View(data);
        }
    }
}
