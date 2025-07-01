using Microsoft.AspNetCore.Mvc;

namespace SV21T1020102.Web.Controllers
{
    public class InventoryTransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Edit()  
        {
            return View();
        }
    }
}
