using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020102.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "product_search"; 
        public IActionResult Index()
        {
            ProductsSearchInput? input = ApplicationContext.GetSessionData<ProductsSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {

                input = new ProductsSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    MaxPrice = 0,
                    MinPrice = 0,
                    CategoryID = 0,
                    SupplierID = 0
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductsSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "",
                                                            input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Create()
        {
            
            Product data = new Product()
            {
                ProductId = 0,
                Photo = "nophoto.png"
            };
            ViewBag.Title = "Bổ sung mặt hàng";
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? _Photo) 
        {
            ViewBag.Title = data.ProductId == 0 ? "Bổ Sung Sản Phẩm" : "Cập Nhật Thông Tin Sản Phẩm ";
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName),"Tên khách hàng không được để trống");
            if (data.CategoryId==0)
                ModelState.AddModelError(nameof(data.CategoryId),"Loại hàng không được để trống");
            if (data.SupplierId == 0)
                ModelState.AddModelError(nameof(data.SupplierId),"Nhà cung cấp không được để trống");        
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit),"Đơn vị tính không được để trống");
            if(data.Price==0)
                ModelState.AddModelError(nameof(data.Price),"Giá hàng không được để trống");
            if (data.Price<0)
                ModelState.AddModelError(nameof(data.Price),"Giá hàng không hợp lệ");

            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\Product", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            if (data.ProductId != 0)
            {
                ProductDataService.UpdateProduct(data);
                          
            }
            else
            {
                ProductDataService.AddProduct(data);
            }
            return RedirectToAction("Index");
        }
       public IActionResult Delete(int id=0)
        {
            ViewBag.Title = "Xóa mặt hàng";
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data =ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
            
        }
        public IActionResult Photo(int id = 0, string method = "", long photoid = 0)
		{
			switch (method)
			{
				case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto productphoto = new ProductPhoto()
                    {
                        ProductId = id,
                        PhotoId=0
                    };                       
					return View("Photo", productphoto);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh cho mặt hàng";
                    var data = ProductDataService.GetProductPhoto(photoid);
                    return View(data);
                case "delete":
                   
                    ProductDataService.DeleteProductPhoto(photoid);

                    return RedirectToAction("Edit", new { id = id });
                default:					
                    return RedirectToAction("Index");
            }	
		}
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? _Photo)
        {
            if (string.IsNullOrWhiteSpace(data.Photo))
                ModelState.AddModelError(nameof(data.Photo),"Vui lòng nhập ảnh sản phẩm ");
            if(string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả sản phẩm ");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng nhập thứ tự sản phẩm");
            if (data.DisplayOrder < 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Số thứ tự không hợp lệ");

            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\Product\ProductPhoto", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (ModelState.IsValid == false)
            {
                return View("Photo", data);
            }
            if (data.PhotoId == 0)
            {
                long id = ProductDataService.AddProductPhoto(data);
                //int intValue = (int)id;
                if (id < 0)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự sản phẩm đã tồn tại ");
                    return View("Photo", data);
                }              
            }
            else
            {
                bool result = ProductDataService.UpdateProductPhoto(data);
                if (result== false) 
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự sản phẩm đã tồn tại ");
                    return View("Photo", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductId });
        }
        public IActionResult Attribute(int id = 0, string method = "", long attributeid = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính  cho mặt hàng";
                    ProductAttribute data = new ProductAttribute()
                    {
                        ProductId = id,
                        AttributeId = 0
                    };
                    return View("Attribute",data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính cho mặt hàng";
                    var dataa = ProductDataService.GetAttribute(attributeid);
                    return View(dataa);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeid);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data) 
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Vui lòng nhập thông tin  sản phẩm ");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị sản phẩm ");
            if (data.DisplayOrder==0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng nhập sô thứ tự sản phẩm ");
            if (data.DisplayOrder < 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Sô thứ tự sản phẩm k hợp ");
            if (ModelState.IsValid == false)
            {
                return View("Attribute", data);
            }
            if (data.AttributeId == 0)
            {
                long id=ProductDataService.AddAttribute(data);
                if (id < 0) 
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự sản phẩm đã tồn tại ");
                    return View("Attribute", data);
                }
            }
            else 
            {
                bool result=ProductDataService.UpdateAttribute(data);
                if (result==false) 
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự sản phẩm đã tồn tại ");
                    return View("Attribute", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductId });
        }

    }
}
