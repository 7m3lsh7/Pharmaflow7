using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Pharmaflow7.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace Pharmaflow7.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new UserRegistrationModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            _logger.LogInformation("📋 Register attempt: Email={Email}, RoleType={RoleType}", model.Email, model.RoleType);

            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Email is required.");
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password", "Password is required.");
            if (!string.IsNullOrEmpty(model.Password) && (model.Password.Length < 8 || !model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsDigit)))
                ModelState.AddModelError("Password", "Password must be at least 8 characters, with an uppercase letter and a number.");
            if (string.IsNullOrEmpty(model.RoleType))
                ModelState.AddModelError("RoleType", "User type is required.");

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
                _logger.LogWarning("❌ ModelState invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

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
                _logger.LogInformation("✅ User created: {Email}", user.Email);
                if (!await _roleManager.RoleExistsAsync(model.RoleType))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.RoleType));
                }
                await _userManager.AddToRoleAsync(user, model.RoleType);

                // إضافة Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleType),
                    new Claim("RoleType", user.RoleType),
                    new Claim("UserName", user.RoleType == "company" ? user.CompanyName : user.RoleType == "distributor" ? user.DistributorName : user.FullName ?? user.Email)
                };
                await _userManager.AddClaimsAsync(user, claims);

                // تسجيل الدخول مع الـ Claims
                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("✅ Signed in user: {Email} with claims: {Claims}", user.Email, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                return RedirectToDashboard(model.RoleType);
            }

            _logger.LogError("❌ User creation failed: {Email}, Errors: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("❌ Invalid login attempt: {Email}", model.Email);
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // تحديث الـ Claims
            var existingClaims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, existingClaims);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.RoleType),
                new Claim("RoleType", user.RoleType),
                new Claim("UserName", user.RoleType == "company" ? user.CompanyName : user.RoleType == "distributor" ? user.DistributorName : user.FullName ?? user.Email)
            };
            await _userManager.AddClaimsAsync(user, claims);

            var result = await _signInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("✅ Login successful: {Email} with claims: {Claims}", model.Email, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                return RedirectToDashboard(user.RoleType);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("❌ User locked out: {Email}", model.Email);
                return RedirectToAction("Lockout");
            }

            _logger.LogWarning("❌ Invalid login attempt: {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl }, protocol: Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            _logger.LogInformation("Starting External Login for {Provider} with redirect URL: {RedirectUrl}", provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                if (remoteError != null)
                {
                    _logger.LogWarning("External Provider error: {Error}", remoteError);
                    return RedirectToAction("Login");
                }

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogError("ExternalLoginInfo is null");
                    return RedirectToAction("Login");
                }

                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("External login successful: {Provider}", info.LoginProvider);
                    var Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var User = await _userManager.FindByEmailAsync(Email);
                    // تحديث الـ Claims
                    var existingClaims = await _userManager.GetClaimsAsync(User);
                    await _userManager.RemoveClaimsAsync(User, existingClaims);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, User.Email),
                        new Claim(ClaimTypes.Role, User.RoleType),
                        new Claim("RoleType", User.RoleType),
                        new Claim("UserName", User.RoleType == "company" ? User.CompanyName : User.RoleType == "distributor" ? User.DistributorName : User.FullName ?? User.Email)
                    };
                    await _userManager.AddClaimsAsync(User, claims);
                    await _signInManager.SignInAsync(User, isPersistent: true);
                    _logger.LogInformation("Claims updated for external login: {Claims}", string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                    return RedirectToDashboard(User.RoleType);
                }

                // إنشاء مستخدم جديد لو مش موجود
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        await _userManager.AddLoginAsync(user, info);
                        return RedirectToAction("CompleteRegistration", new { email });
                    }
                    else
                    {
                        _logger.LogError("Failed to create user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        ModelState.AddModelError("", "Failed to create user account.");
                        return RedirectToAction("Login");
                    }
                }
                else if (string.IsNullOrEmpty(user.RoleType))
                {
                    return RedirectToAction("CompleteRegistration", new { email });
                }

                // تحديث الـ Claims للمستخدم الموجود
                var userClaims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, userClaims);
                var newClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleType),
                    new Claim("RoleType", user.RoleType),
                    new Claim("UserName", user.RoleType == "company" ? user.CompanyName : user.RoleType == "distributor" ? user.DistributorName : user.FullName ?? user.Email)
                };
                await _userManager.AddClaimsAsync(user, newClaims);
                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("External login signed in: {Email} with claims: {Claims}", user.Email, string.Join(", ", newClaims.Select(c => $"{c.Type}: {c.Value}")));
                return RedirectToDashboard(user.RoleType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExternalLoginCallback: {Message}", ex.Message);
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CompleteRegistration(string email)
        {
            return View(new UserRegistrationModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CompleteRegistration(UserRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ModelState for CompleteRegistration: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for CompleteRegistration: {Email}", model.Email);
                return RedirectToAction("Login");
            }

            user.RoleType = model.RoleType;
            user.FullName = model.FullName;
            user.Address = model.Address;
            user.CompanyName = model.CompanyName;
            user.LicenseNumber = model.LicenseNumber;
            user.ContactNumber = model.RoleType == "company" ? model.CompanyContactNumber : model.RoleType == "distributor" ? model.DistributorContactNumber : null;
            user.DistributorName = model.DistributorName;
            user.WarehouseAddress = model.WarehouseAddress;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(model.RoleType))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.RoleType));
                }
                await _userManager.AddToRoleAsync(user, model.RoleType);

                // إضافة Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleType),
                    new Claim("RoleType", user.RoleType),
                    new Claim("UserName", user.RoleType == "company" ? user.CompanyName : user.RoleType == "distributor" ? user.DistributorName : user.FullName ?? user.Email)
                };
                await _userManager.AddClaimsAsync(user, claims);

                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("CompleteRegistration successful: {Email} with claims: {Claims}", user.Email, string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                return RedirectToDashboard(model.RoleType);
            }

            _logger.LogError("CompleteRegistration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
            _logger.LogInformation("✅ User logged out");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private IActionResult RedirectToDashboard(string roleType)
        {
            string roleTypeLower = roleType?.ToLower() ?? "home";
            _logger.LogInformation("Redirecting to dashboard for role: {RoleType}", roleTypeLower);
            return RedirectToAction(roleTypeLower switch
            {
                "consumer" => "ConsumerDashboard",
                "company" => "CompanyDashboard",
                "distributor" => "Dashboard",
                _ => "Index"
            }, roleTypeLower switch
            {
                "consumer" => "Consumer",
                "company" => "Company",
                "distributor" => "Distributor",
                _ => "Home_page"
            });
        }
    }
}