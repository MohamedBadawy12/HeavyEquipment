using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Users.Dtos;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Application.Features.Users.Commands.Queries
{
    public record GetAllUsersQuery(
         string? RoleFilter = null,
         string? Search = null
     ) : IRequest<Result<IReadOnlyList<UserDto>>>;

    public class GetAllUsersHandler
       : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
    {
        private readonly UserManager<AppUser> _userManager;
        public GetAllUsersHandler(UserManager<AppUser> userManager) => _userManager = userManager;

        public async Task<Result<IReadOnlyList<UserDto>>> Handle(
            GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrEmpty(request.RoleFilter) &&
                Enum.TryParse<UserType>(request.RoleFilter, out var roleEnum))
                query = query.Where(u => u.Role == roleEnum);

            if (!string.IsNullOrEmpty(request.Search))
                query = query.Where(u =>
                    u.FullName.Contains(request.Search) ||
                    (u.Email != null && u.Email.Contains(request.Search)));

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto(
                    u.Id, u.FullName, u.Email ?? "",
                    u.PhoneNumber ?? "", u.Role.ToString(),
                    u.TrustScore, u.IsVerified, u.IsBlocked, u.CreatedAt))
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<UserDto>>.Success(users);
        }
    }
}
