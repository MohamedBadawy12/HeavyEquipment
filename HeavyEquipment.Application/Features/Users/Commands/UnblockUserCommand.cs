using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Users.Commands
{
    public record UnblockUserCommand(Guid UserId) : IRequest<Result>;

    public class UnblockUserHandler : IRequestHandler<UnblockUserCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        public UnblockUserHandler(UserManager<AppUser> um) => _userManager = um;

        public async Task<Result> Handle(UnblockUserCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null) return Result.Failure("المستخدم غير موجود");
            if (!user.IsBlocked) return Result.Failure("المستخدم غير محظور أصلاً");
            user.Unblock();
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
    }
}
