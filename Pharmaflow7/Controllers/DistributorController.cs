using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmaflow7.Data;
using Pharmaflow7.Models;
using System.Threading.Tasks;

namespace Pharmaflow7.Controllers
{
    [Authorize(Roles = "distributor")]
    public class DistributorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // البناء (Constructor)
        public DistributorController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // دالة Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // عرض الشحنات المعينة للموزع
        [HttpGet]
        public async Task<IActionResult> TrackShipments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "distributor")
            {
                return RedirectToAction("Login", "Auth");
            }

            string distributorId = user.Id;
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
        }

        // تحديث موقع الشحنة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipmentLocation(int id, double lat, double lng)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.RoleType != "distributor")
            {
                return RedirectToAction("Login", "Auth");
            }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == id && s.DistributorId == user.Id);
            if (shipment == null)
            {
                return NotFound();
            }

            shipment.CurrentLocationLat = lat;
            shipment.CurrentLocationLng = lng;
            shipment.Status = "In Transit"; // تحديث الحالة حسب الحاجة
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Location updated successfully!" });
        }

        // إدارة المخزون
        public IActionResult InventoryManagement() // تغيير الاسم إلى InventoryManagement لاتباع تسمية C#
        {
            return View();
        }
    }
}