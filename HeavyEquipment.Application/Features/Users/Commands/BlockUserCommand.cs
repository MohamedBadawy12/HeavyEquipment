using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Application.Features.Users.Commands
{
    public record BlockUserCommand(Guid UserId) : IRequest<Result>;

    public class BlockUserHandler : IRequestHandler<BlockUserCommand, Result>
    {
        private readonly UserManager<AppUser> _userManager;
        public BlockUserHandler(UserManager<AppUser> um) => _userManager = um;

        public async Task<Result> Handle(BlockUserCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null) return Result.Failure("المستخدم غير موجود");
            if (user.IsBlocked) return Result.Failure("المستخدم محظور مسبقاً");
            user.Block();
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
    }
}
