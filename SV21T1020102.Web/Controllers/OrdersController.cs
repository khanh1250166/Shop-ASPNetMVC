using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020102.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class OrdersController : Controller
	{
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;
        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchSale";
        private const string SHOPPING_CART = "ShoppingCart";

        public IActionResult Index()
		{
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null) 
            {
                var cultureInfo= new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status=0,
                    TimeRange=$"{DateTime.Today.AddDays(-10000).ToString("dd/MM/yyyy",cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }

			return View(condition);
		}
        public IActionResult Search(OrderSearchInput condition) 
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page,condition.PageSize, condition.SearchValue ?? "", condition.Status,condition.FromTime,condition.ToTime);
            var model = new OrderSearchResult()
            {
                Page=condition.Page,
                PageSize=condition.PageSize,
                SearchValue=condition.SearchValue??"",
                Status=condition.Status,
                TimeRange=condition.TimeRange,
                RowCount=rowCount,
                Data=data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }
		public IActionResult Create() {
            var condition = ApplicationContext.GetSessionData<ProductsSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductsSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
			return View(condition); 
		}
        public IActionResult SearchProduct(ProductsSearchInput condition) 
        {

            int rowCount;
            var data = ProductDataService.ListOfProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order=OrderDataService.GetOrder(id);
            if (order == null)           
                return RedirectToAction("Index");
            var detail = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details=detail
            };    

            return View(model);
        }
        [HttpGet]
        public IActionResult EditDetails(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateDetails(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json("Số lượng bán không hợp lệ");
            if (salePrice <= 0)
                return Json("Giá bán không hợp lệ");
            bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            if (!result)
                return Json("Không được phép thay đổi thông tin của đơn hàng này!");
            return Json("");
        }
        public IActionResult DeleteDetails(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";
            return RedirectToAction("Details", new { id = id });
        }
       
		public IActionResult Shipping(int id = 0)
		{
			// Truyền dữ liệu vào ViewModel
			var model = new ShippingViewModel
			{
				OrderID = id,
				ShipperID = 0,
				Message = null // Ban đầu không có thông báo lỗi
			};
			return View(model);
		}

		// POST: Shipping
		[HttpPost]
		public IActionResult Shipping(ShippingViewModel model)
		{
			if (model.ShipperID <= 0)
			{
				// Truyền thông báo lỗi qua Model
				model.Message = "Vui lòng chọn người giao hàng!";
				return View(model);
			}

			// Thực hiện logic xử lý gán shipper
			bool result = OrderDataService.ShipOrder(model.OrderID, model.ShipperID);

			if (!result)
			{
				// Nếu xảy ra lỗi, truyền thông báo qua Model
				model.Message = "Đơn hàng không cho phép chuyển người giao hàng!";
				return View(model);
			}

			// Nếu thành công, điều hướng đến trang chi tiết đơn hàng
			return RedirectToAction("Details", new { id = model.OrderID });
		}
		private List<CartItem> GetShoppingCart() 
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (cart == null) 
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }
        public IActionResult AddToCart(CartItem item) 
        {
            if(item.SalePrice<0|| item.Quantity<=0)
                return Json("Giá bán k hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id=0) 
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductId == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart() 
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart); 
            return Json("");
        }
        public IActionResult ShoppingCart() 
        { 
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID=0,string deliveryProvince="",string deliveryAddress="") 
        {
           var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count() == 0)
                return Json("Giỏ hàng trống, vui lòng chọn mặt hàng");
            var userData = User.GetUserData();

                var employyeID = int.Parse(userData.UserId); 
 

            if (customerID == 0|| string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin!");
            }
          
            List<OrderDetail>orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart) 
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID=item.ProductId,
                    Quantity=item.Quantity, 
                    SalePrice=item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employyeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
        public IActionResult Delete(int id) 
        {
            bool model = OrderDataService.DeleteOrder(id);
            if(!model)
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Accept(int id=0) 
        {
            var userData = User.GetUserData();

            var employyeID = int.Parse(userData.UserId);
            bool model = OrderDataService.AcceptOrder(id, employyeID);
            if (!model)
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Finish(int id = 0) 
        {
            bool model = OrderDataService.FinishOrder(id);
            if (!model)
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id = id });
        }
            public IActionResult Cancel(int id = 0) 
            {
                bool model = OrderDataService.CancelOrder(id);
                if (!model)
                    TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";

                return RedirectToAction("Details", new { id = id });
            }
        public IActionResult Reject(int id = 0) 
        {
            bool model = OrderDataService.RejectOrder(id);
            if (!model)
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";

            return RedirectToAction("Details", new { id = id });
        }
    }
}

