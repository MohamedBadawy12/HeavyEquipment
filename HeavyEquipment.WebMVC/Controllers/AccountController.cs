using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.WebMVC.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register(string? role)
        {
            ViewBag.Role = role ?? "Customer";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userRole = model.Role == "Owner" ? UserType.Owner : UserType.Customer;

            var user = new AppUser(
                model.FullName,
                model.Email,
                model.PhoneNumber,
                model.NationalId,
                userRole);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, userRole.ToString());

            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["Success"] = $"أهلاً {user.FullName}! تم إنشاء حسابك بنجاح.";

            return userRole == UserType.Owner
                ? RedirectToAction("Index", "Dashboard")
                : RedirectToAction("Index", "Home");
        }

        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null && user.IsBlocked)
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError("", "تم حظر حسابك. تواصل مع الإدارة للمزيد من المعلومات.");
                    return View(model);
                }

                TempData["Success"] = "تم تسجيل الدخول بنجاح!";
                return LocalRedirect(returnUrl ?? "/");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "تم تعليق الحساب مؤقتاً بسبب محاولات تسجيل دخول متعددة.");
                return View(model);
            }

            ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction(nameof(Login));
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction(nameof(Login));

            try
            {
                user.UpdateProfile(fullName, phoneNumber);
                await _userManager.UpdateAsync(user);
                TempData["ProfileSuccess"] = "تم تحديث بياناتك بنجاح";
            }
            catch (DomainException ex)
            {
                TempData["ProfileError"] = ex.Message;
            }

            return RedirectToAction(nameof(Profile));
        }
    }
}
