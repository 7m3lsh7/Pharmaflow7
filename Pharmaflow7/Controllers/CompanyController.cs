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
        // عرض قائمة المنتجات
        [HttpGet]
        public async Task<IActionResult> ManageProducts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            var products = await _context.Products
                .Where(p => p.CompanyId == user.Id)
                .ToListAsync();

            return View(products);
        }

        // عرض نموذج تعديل المنتج
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == user.Id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ProductionDate = product.ProductionDate,
                ExpirationDate = product.ExpirationDate,
                Description = product.Description
            };

            return PartialView("_EditProductModal", model); // إرجاع نموذج Modal كـ Partial View
        }

        // معالجة تعديل المنتج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_EditProductModal", model);
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.CompanyId == user.Id);

            if (product == null)
            {
                return NotFound();
            }

            if (model.ProductionDate >= model.ExpirationDate)
            {
                ModelState.AddModelError("ExpirationDate", "Expiration date must be after production date.");
                return PartialView("_EditProductModal", model);
            }

            product.Name = model.Name;
            product.ProductionDate = model.ProductionDate;
            product.ExpirationDate = model.ExpirationDate;
            product.Description = model.Description;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Product updated successfully!" });
        }

        // حذف المنتج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == user.Id);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // إرجاع رد JSON مع معرف السطر ليتم إزالته ديناميكيًا
            return Json(new { success = true, productId = id, message = "Product deleted successfully!" });
        }


        [HttpGet]
        public async Task<IActionResult> CreateShipment()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new ShipmentViewModel
            {
                Products = await _context.Products.Where(p => p.CompanyId == user.Id).ToListAsync(),
                Distributors = await _userManager.GetUsersInRoleAsync("distributor")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipment(ShipmentViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                model.Products = await _context.Products.Where(p => p.CompanyId == user.Id).ToListAsync();
                model.Distributors = await _userManager.GetUsersInRoleAsync("distributor");
                return View(model);
            }

            var shipment = new Shipment
            {
                ProductId = model.ProductId,
                Destination = model.Destination,
                Status = "Pending",
                CompanyId = user.Id,
                DistributorId = model.DistributorId,
                CurrentLocationLat = model.LocationLat,
                CurrentLocationLng = model.LocationLng
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            return RedirectToAction("TrackShipments");
        }

        [HttpGet]
        public async Task<IActionResult> TrackShipments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "company")
            {
                return RedirectToAction("Login", "Auth");
            }

            int companyId = user.Id;
            var shipments = await _context.Shipments
                .Where(s => s.CompanyId == companyId)
                .Include(s => s.Product)
                .Include(s => s.Distributor)
                .Select(s => new ShipmentViewModel
                {
                    Id = s.Id,
                    ProductName = s.Product.Name,
                    Destination = s.Destination,
                    Status = s.Status,
                    CurrentLocation = $"{s.CurrentLocationLat}, {s.CurrentLocationLng}",
                    LocationLat = s.CurrentLocationLat,
                    LocationLng = s.CurrentLocationLng,
                    DistributorName = s.Distributor != null ? s.Distributor.UserName : "Not Assigned"
                })
                .ToListAsync();

            return View(shipments);
        }


        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "company")]
        public async Task<IActionResult> AddProduct([FromBody]  Product model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType.ToLower() != "company")
            {
                return Unauthorized();
            }

            Console.WriteLine($"Received data - Name: {model.Name}, Description: {model.Description}, ProductionDate: {model.ProductionDate}, ExpirationDate: {model.ExpirationDate}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Field: {error.Key}, Error: {error.Value.Errors.First().ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = model.Name,
                ProductionDate = model.ProductionDate,
                ExpirationDate = model.ExpirationDate,
                Description = model.Description ?? string.Empty,
                CompanyId = user.Id
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Json(new { productId = product.Id });
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
     
    }
}