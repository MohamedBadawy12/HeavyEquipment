using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Users.Commands
{
    public record VerifyUserCommand(Guid UserId) : IRequest<Result>;

    public class VerifyUserHandler : IRequestHandler<VerifyUserCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        public VerifyUserHandler(UserManager<AppUser> um) => _userManager = um;

        public async Task<Result> Handle(VerifyUserCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null) return Result.Failure("المستخدم غير موجود");
            if (user.IsVerified) return Result.Failure("المستخدم موثق مسبقاً");
            user.Verify();
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
    }
}
