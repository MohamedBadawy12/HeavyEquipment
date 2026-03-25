using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Application.Features.Users.Commands
{
    public record ResetPasswordCommand(string PhoneNumber, string Code, string NewPassword) : IRequest<Result>;

    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordHandler(UserManager<AppUser> userManager) => _userManager = userManager;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

            if (user == null || user.VerificationCode != request.Code)
                return Result.Failure("بيانات غير صحيحة");

            if (user.CodeExpiry < DateTime.UtcNow)
                return Result.Failure("انتهت صلاحية الكود");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
            {
                user.ClearVerificationCode();
                await _userManager.UpdateAsync(user);
                return Result.Success();
            }

            return Result.Failure("حدث خطأ أثناء تغيير كلمة المرور");
        }
    }
}
