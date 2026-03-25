using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Application.Features.Users.Commands
{
    public record ForgotPasswordCommand(string PhoneNumber) : IRequest<Result>;

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager; // لو شغال Identity
        private readonly ISmsService _smsService;

        public ForgotPasswordHandler(UserManager<AppUser> userManager, ISmsService smsService)
        {
            _userManager = userManager;
            _smsService = smsService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null) return Result.Failure("المستخدم غير موجود");

            var code = new Random().Next(100000, 999999).ToString();

            user.SetVerificationCode(code);

            await _userManager.UpdateAsync(user);

            var message = $"كود إعادة تعيين كلمة المرور هو: {code}";
            var sent = await _smsService.SendSmsAsync(user.PhoneNumber, message, cancellationToken);

            return sent ? Result.Success() : Result.Failure("فشل إرسال الرسالة، تأكد من الرقم");
        }
    }
}
