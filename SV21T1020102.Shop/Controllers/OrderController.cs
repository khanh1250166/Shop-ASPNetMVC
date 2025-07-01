using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.DomainModels;
using SV21T1020102.Shop.Models;

namespace SV21T1020102.Shop.Controllers
{
	public class OrderController : Controller
	{
		private const string SHOPPING_CART = "ShoppingCart";

		// Lấy danh sách sản phẩm trong giỏ hàng
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

		// Hiển thị giỏ hàng
		public IActionResult Index()
		{
			var shoppingCart = GetShoppingCart();
			return View(shoppingCart);
		}
		public IActionResult ShoppingCart()
		{
			return View(GetShoppingCart());
		}
        // Thêm sản phẩm vào giỏ
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
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
            return Json(new {
                status = "success",
                message = "Sản phẩm" +item.ProductName+" đã được thêm vào giỏ hàng! Số lượng:"+ item.Quantity
            });
        }

        // Xóa một sản phẩm khỏi giỏ
        public IActionResult RemoveFromCart(int id = 0)
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
        public IActionResult Init(int customerID, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count() == 0)
                return Json("Giỏ hàng trống, vui lòng chọn mặt hàng");
			var userData = User.GetUserData();

			 customerID = int.Parse(userData.UserId);

			int? employyeID = null;
            if (string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin!");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employyeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
    }
}