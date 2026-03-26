using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;
using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.WebMVC.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeavyEquipment.WebMVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMediator _mediator;
        private readonly ISmsService _smsService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IMediator mediator,
            IUnitOfWork unitOfWork, ISmsService smsService) : base(unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _smsService = smsService;
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

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string phoneNumber)
        {
            var user = _userManager.Users
                .FirstOrDefault(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);

            if (user is null)
            {
                TempData["Info"] = "لو الرقم مسجل، هيوصلك كود خلال ثوان";
                return RedirectToAction(nameof(VerifyResetCode));
            }

            var code = new Random().Next(100000, 999999).ToString();
            var purpose = "ResetPassword";

            await _userManager.RemoveAuthenticationTokenAsync(user, "Phone", purpose);
            await _userManager.SetAuthenticationTokenAsync(user, "Phone", purpose, code);

            var message = $"كود إعادة تعيين كلمة مرور HeavyHub: {code} — صالح لمدة 10 دقائق";
            await _smsService.SendAsync(user.PhoneNumber!, message);

            TempData["ResetPhone"] = phoneNumber;
            TempData["Info"] = "تم إرسال كود التحقق على رقمك";
            return RedirectToAction(nameof(VerifyResetCode));
        }

        public IActionResult VerifyResetCode()
        {
            ViewBag.Phone = TempData["ResetPhone"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResetCode(string phoneNumber, string code)
        {
            var user = _userManager.Users
                .FirstOrDefault(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);

            if (user is null)
            {
                ModelState.AddModelError("", "رقم الهاتف غير صحيح");
                return View();
            }

            var savedCode = await _userManager.GetAuthenticationTokenAsync(user, "Phone", "ResetPassword");

            if (savedCode != code)
            {
                ModelState.AddModelError("", "الكود غير صحيح أو منتهي الصلاحية");
                return View();
            }

            TempData["ResetPhone"] = phoneNumber;
            TempData["ResetCode"] = code;
            return RedirectToAction(nameof(ResetPassword));
        }

        public IActionResult ResetPassword()
        {
            ViewBag.Phone = TempData.Peek("ResetPhone");
            ViewBag.Code = TempData.Peek("ResetCode");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            string phoneNumber, string code,
            string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "كلمتا المرور غير متطابقتين");
                return View();
            }

            var user = _userManager.Users
                .FirstOrDefault(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);

            if (user is null)
            {
                ModelState.AddModelError("", "حدث خطأ. حاول مرة أخرى");
                return View();
            }

            var savedCode = await _userManager.GetAuthenticationTokenAsync(user, "Phone", "ResetPassword");
            if (savedCode != code)
            {
                ModelState.AddModelError("", "انتهت صلاحية الكود. حاول مرة أخرى");
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View();
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "Phone", "ResetPassword");

            TempData["Success"] = "تم تغيير كلمة المرور بنجاح! يمكنك تسجيل الدخول الآن.";
            return RedirectToAction(nameof(Login));
        }
    }
}
