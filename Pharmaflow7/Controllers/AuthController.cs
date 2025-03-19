using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Pharmaflow7.Models;

namespace Pharmaflow7.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegistrationModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            // تسجيل البيانات المُرسلة للتحقق
            Console.WriteLine("📋 البيانات المُرسلة:");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Password: {model.Password}");
            Console.WriteLine($"RoleType: {model.RoleType}");
            Console.WriteLine($"FullName: {model.FullName}");
            Console.WriteLine($"Address: {model.Address}");
            Console.WriteLine($"CompanyName: {model.CompanyName}");
            Console.WriteLine($"LicenseNumber: {model.LicenseNumber}");
            Console.WriteLine($"ContactNumber: {model.ContactNumber}");
            Console.WriteLine($"DistributorName: {model.DistributorName}");
            Console.WriteLine($"WarehouseAddress: {model.WarehouseAddress}");

            // مسح أي أخطاء موجودة في ModelState لنبدأ من جديد
            ModelState.Clear();

            // التحقق من الحقول الأساسية (Email, Password, RoleType)
            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email is required.");
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password", "Password is required.");
            if (string.IsNullOrEmpty(model.RoleType))
                ModelState.AddModelError("RoleType", "User type is required.");

            // تحقق مخصص بناءً على نوع المستخدم
            if (model.RoleType == "consumer")
            {
                if (string.IsNullOrEmpty(model.FullName))
                    ModelState.AddModelError("FullName", "Full Name is required for consumers.");
                // لا نتحقق من حقول الشركة أو الموزع
            }
            else if (model.RoleType == "company")
            {
                if (string.IsNullOrEmpty(model.CompanyName))
                    ModelState.AddModelError("CompanyName", "Company Name is required for companies.");
                if (string.IsNullOrEmpty(model.LicenseNumber))
                    ModelState.AddModelError("LicenseNumber", "License Number is required for companies.");
                if (string.IsNullOrEmpty(model.ContactNumber))
                    ModelState.AddModelError("ContactNumber", "Contact Number is required for companies.");
                // لا نتحقق من FullName أو DistributorName أو WarehouseAddress
            }
            else if (model.RoleType == "distributor")
            {
                if (string.IsNullOrEmpty(model.DistributorName))
                    ModelState.AddModelError("DistributorName", "Distributor Name is required for distributors.");
                if (string.IsNullOrEmpty(model.WarehouseAddress))
                    ModelState.AddModelError("WarehouseAddress", "Warehouse Address is required for distributors.");
                if (string.IsNullOrEmpty(model.ContactNumber))
                    ModelState.AddModelError("ContactNumber", "Contact Number is required for distributors.");
                // لا نتحقق من FullName أو CompanyName
            }
            else if (!string.IsNullOrEmpty(model.RoleType))
            {
                ModelState.AddModelError("RoleType", "Invalid user type.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState غير صالح");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"الحقل: {error.Key}");
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"- خطأ: {err.ErrorMessage}");
                    }
                }
                return View(model);
            }

            Console.WriteLine("✅ بدأ إنشاء المستخدم...");
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                RoleType = model.RoleType,
                FullName = model.FullName,
                Address = model.Address,
                CompanyName = model.CompanyName,
                LicenseNumber = model.LicenseNumber,
                ContactNumber = model.ContactNumber,
                DistributorName = model.DistributorName,
                WarehouseAddress = model.WarehouseAddress
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("✅ تم إنشاء المستخدم بنجاح!");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(model.RoleType switch
                {
                    "consumer" => "ConsumerDashboard",
                    "company" => "CompanyDashboard",
                    "distributor" => "DistributorDashboard",
                    _ => "Index"
                }, "Home");
            }

            Console.WriteLine("❌ فشل إنشاء المستخدم:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Code}: {error.Description}");
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}