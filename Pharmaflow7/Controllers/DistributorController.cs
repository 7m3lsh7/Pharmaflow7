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
    [Authorize(Roles = "distributor")]
    public class DistributorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DistributorController(AppDbContext context, UserManager<ApplicationUser> userManager)
        public IActionResult dashboard()
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> TrackShipments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "distributor")
            {
                return RedirectToAction("Login", "Auth");
            }

            int distributorId = user.Id;
            var shipments = await _context.Shipments
                .Where(s => s.DistributorId == distributorId)
                .Include(s => s.Product)
                .Select(s => new ShipmentViewModel
                {
                    Id = s.Id,
                    ProductName = s.Product.Name,
                    Destination = s.Destination,
                    Status = s.Status,
                    CurrentLocation = $"{s.CurrentLocationLat}, {s.CurrentLocationLng}",
                    LocationLat = s.CurrentLocationLat,
                    LocationLng = s.CurrentLocationLng
                })
                .ToListAsync();

            return View(shipments);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipmentLocation(int id, double lat, double lng)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "distributor")
        public IActionResult Inventory_Management()
        {
                return RedirectToAction("Login", "Auth");
            return View();
        }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == id && s.DistributorId == user.Id);
            if (shipment == null)
             public IActionResult Track_Shipments()
        {
                return NotFound();
            }

            shipment.CurrentLocationLat = lat;
            shipment.CurrentLocationLng = lng;
            shipment.Status = "In Transit"; // تحديث الحالة حسب الحاجة
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Location updated successfully!" });
            return View();
        }
    }
}
