using Microsoft.AspNetCore.Mvc;
using SV21T1020102.BusineesLayers;
using SV21T1020102.Shop.Models;


namespace SV21T1020102.Shop.Controllers
{
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "product_search";
        private const string SHOPPING_CART = "ShoppingCart";
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
        public IActionResult Details(int id)
        {
            var data = ProductDataService.GetProduct(id);
            return View(data);
        }
    }
}
