using Microsoft.AspNetCore.Mvc;

namespace Pharmaflow7.Controllers
{
    public class DistributorController : Controller
    {
        public IActionResult dashboard()
        {
            return View();
        }
        public IActionResult Inventory_Management()
        {
            return View();
        }
             public IActionResult Track_Shipments()
        {
            return View();
        }
             public IActionResult report()
        {
            return View();
        }
    }
}
