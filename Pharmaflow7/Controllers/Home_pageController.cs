using Microsoft.AspNetCore.Mvc;

namespace Pharmaflow7.Controllers
{
    public class Home_pageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
