using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pharmaflow7.Models;

namespace Pharmaflow7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
                var model = new DashboardViewModel
                {
                    RegisteredMedicines = 200,
                    ActiveShipments = 100,
                    LowStockMedicines = 500,
                    DailyShipmentRate = 200
                };
                return View(model);
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
