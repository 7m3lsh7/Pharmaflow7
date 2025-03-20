using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmaflow7.Data;
using Pharmaflow7.Models; // للوصول إلى CompanyDashboardViewModel
using System.Linq;
using System.Threading.Tasks;

namespace Pharmaflow7.Controllers
{
    [Authorize(Roles = "company")]
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CompanyController(AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CompanyDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType.ToLower() != "company")
            {
                return RedirectToAction("Register", "Auth");
            }

            var totalProducts = await _context.Products
                .CountAsync(p => p.CompanyId == user.Id);

            var activeShipments = await _context.Shipments
                .Where(s => s.CompanyId == user.Id && s.Status != "Delivered")
                .CountAsync();

            var performanceScore = 85;

            var shipments = await _context.Shipments
                .Where(s => s.CompanyId == user.Id && s.Status != "Delivered")
                .Join(_context.Products,
                      s => s.ProductId,
                      p => p.Id,
                      (s, p) => new { s.Id, ProductName = p.Name, s.Destination, s.Status })
                .ToListAsync();

            var salesData = await _context.Shipments
                .Where(s => s.CompanyId == user.Id)
                .GroupBy(s => s.CreatedDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Count() })
                .ToListAsync();

            var distributionData = await _context.Shipments
                .Where(s => s.CompanyId == user.Id)
                .GroupBy(s => s.Destination)
                .Select(g => new { Destination = g.Key, Count = g.Count() })
                .ToListAsync();

            var viewModel = new CompanyDashboardViewModel
            {
                TotalProducts = totalProducts,
                ActiveShipments = activeShipments,
                PerformanceScore = performanceScore,
                Shipments = shipments.Cast<object>().ToList(),
                SalesData = salesData.Cast<object>().ToList(),
                DistributionData = distributionData.Cast<object>().ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ManageProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Register", "Auth");
            }

            var products = await _context.Products
                .Where(p => p.CompanyId == user.Id)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(string name, string description)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Register", "Auth");
            }

            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Product name is required.");
                return View();
            }

            var product = new Product
            {
                Name = name,
                Description = description ?? string.Empty,
                CompanyId = user.Id
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageProducts");
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Register", "Auth");
            }

            var salesData = await _context.Shipments
                .Where(s => s.CompanyId == user.Id)
                .GroupBy(s => s.CreatedDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Count() })
                .ToListAsync();

            return View(salesData);
        }
        public IActionResult tracking()
        {
            return View();
        }
    }
}