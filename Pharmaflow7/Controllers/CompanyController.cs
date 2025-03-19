using Microsoft.AspNetCore.Mvc;

namespace Pharmaflow7.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult manage_product()
        {
            return View();
        }
        public IActionResult add_product()
        {
            return View();
        }
        public IActionResult reports()
        {
            return View();
        }
        public IActionResult tracking()
        {
            return View();
        }
    }
}
