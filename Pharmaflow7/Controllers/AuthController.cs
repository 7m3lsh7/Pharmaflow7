using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Pharmaflow7.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Pharmaflow7.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // عرض صفحة التسجيلnm
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new UserRegistrationModel());
        }

        // معالجة طلب التسجيل
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            // تسجيل البيانات المُرسلة للتحقق (Debugging)
            Console.WriteLine("📋 البيانات المُرسلة:");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Password: {model.Password}");
            Console.WriteLine($"RoleType: {model.RoleType}");
            Console.WriteLine($"FullName: {model.FullName}");
            Console.WriteLine($"Address: {model.Address}");
            Console.WriteLine($"CompanyName: {model.CompanyName}");
            Console.WriteLine($"LicenseNumber: {model.LicenseNumber}");
            Console.WriteLine($"ContactNumber: {model.CompanyContactNumber}");
            Console.WriteLine($"ContactNumber: {model.DistributorContactNumber}");
            Console.WriteLine($"DistributorName: {model.DistributorName}");
            Console.WriteLine($"WarehouseAddress: {model.WarehouseAddress}");

            // مسح أي أخطاء موجودة في ModelState
            ModelState.Clear();

            // التحقق من الحقول الأساسية
            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email is required.");
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password", "Password is required.");
            if (string.IsNullOrEmpty(model.RoleType))
                ModelState.AddModelError("RoleType", "User type is required.");

            // التحقق بناءً على نوع المستخدم
            switch (model.RoleType)
            {
                case "consumer":
                    if (string.IsNullOrEmpty(model.FullName))
                        ModelState.AddModelError("FullName", "Full Name is required for consumers.");
                    break;
                case "company":
                    if (string.IsNullOrEmpty(model.CompanyName))
                        ModelState.AddModelError("CompanyName", "Company Name is required for companies.");
                    if (string.IsNullOrEmpty(model.LicenseNumber))
                        ModelState.AddModelError("LicenseNumber", "License Number is required for companies.");
                    if (string.IsNullOrEmpty(model.CompanyContactNumber))
                        ModelState.AddModelError("CompanyContactNumber", "Contact Number is required for companies.");
                    break;
                case "distributor":
                    if (string.IsNullOrEmpty(model.DistributorName))
                        ModelState.AddModelError("DistributorName", "Distributor Name is required for distributors.");
                    if (string.IsNullOrEmpty(model.WarehouseAddress))
                        ModelState.AddModelError("WarehouseAddress", "Warehouse Address is required for distributors.");
                    if (string.IsNullOrEmpty(model.DistributorContactNumber))
                        ModelState.AddModelError("DistributorContactNumber", "Contact Number is required for distributors.");
                    break;
                default:
                    if (!string.IsNullOrEmpty(model.RoleType))
                        ModelState.AddModelError("RoleType", "Invalid user type.");
                    break;
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
                ContactNumber = model.RoleType == "company" ? model.CompanyContactNumber : model.RoleType == "distributor" ? model.DistributorContactNumber : null,
                DistributorName = model.DistributorName,
                WarehouseAddress = model.WarehouseAddress
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "company"); // تأكدي من هذا السطر
            }

            if (result.Succeeded)
            {
                Console.WriteLine("✅ تم إنشاء المستخدم بنجاح!");
                // إضافة الدور بناءً على RoleType
                string roleType = model.RoleType; // "company" من النموذج
                if (!await _roleManager.RoleExistsAsync(roleType))
                {
                    var role = new IdentityRole(roleType);
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, model.RoleType);
                await _signInManager.SignInAsync(user, isPersistent: false);
                string roleTypeLower = model.RoleType.ToLower();
                return RedirectToAction(roleTypeLower switch
                {
                    "consumer" => "ConsumerDashboard",
                    "company" => "CompanyDashboard",
                    "distributor" => "dashboard",
                    _ => "Index"
                }, roleTypeLower switch
                {
                    "consumer" => "Consumer",
                    "company" => "Company",
                    "distributor" => "Distributor",
                    _ => "Home"
                });
            
        }

            Console.WriteLine("❌ فشل إنشاء المستخدم:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Code}: {error.Description}");
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // عرض صفحة تسجيل الدخول
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // معالجة طلب تسجيل الدخول
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    string roleTypeLower = user.RoleType.ToLower();
                    return RedirectToAction(roleTypeLower switch
                    {
                        "consumer" => "ConsumerDashboard",
                        "company" => "CompanyDashboard",
                        "distributor" => "dashboard",
                        _ => "Index"
                    }, roleTypeLower switch
                    {
                        "consumer" => "Consumer",
                        "company" => "Company",
                        "distributor" => "Distributor",
                        _ => "Home"
                    });
                }
                // إذا لم يتم العثور على المستخدم (نادر الحدوث بعد تسجيل الدخول الناجح)، انتقل إلى الصفحة الرئيسية
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        // تسجيل الخروج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        // مساعد لإعادة التوجيه إلى ReturnUrl إذا كان صالحًا
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }

   
}
