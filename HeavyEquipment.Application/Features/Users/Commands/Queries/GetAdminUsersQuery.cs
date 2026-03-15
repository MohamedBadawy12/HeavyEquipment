using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Users.Dtos;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Application.Features.Users.Commands.Queries
{
    public record GetAdminUsersQuery(
        string? RoleFilter = null,
        string? Search = null
    ) : IRequest<Result<AdminUsersDto>>;

    public class GetAdminUsersHandler
        : IRequestHandler<GetAdminUsersQuery, Result<AdminUsersDto>>
    {
        private readonly UserManager<AppUser> _userManager;

        public GetAdminUsersHandler(UserManager<AppUser> userManager)
            => _userManager = userManager;

        public async Task<Result<AdminUsersDto>> Handle(
            GetAdminUsersQuery request, CancellationToken ct)
        {
            var query = _userManager.Users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrEmpty(request.RoleFilter) &&
                Enum.TryParse<UserType>(request.RoleFilter, out var roleEnum))
                query = query.Where(u => u.Role == roleEnum);

            if (!string.IsNullOrEmpty(request.Search))
                query = query.Where(u =>
                    u.FullName.Contains(request.Search) ||
                    (u.Email != null && u.Email.Contains(request.Search)));

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync(ct);

            var rows = users.Select(u => new AdminUserRow
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                Phone = u.PhoneNumber ?? "",
                Role = u.Role.ToString(),
                TrustScore = u.TrustScore,
                IsVerified = u.IsVerified,
                IsBlocked = u.IsBlocked,
                CreatedAt = u.CreatedAt
            }).ToList();

            var vm = new AdminUsersDto
            {
                Users = rows,
                RoleFilter = request.RoleFilter,
                Search = request.Search,
                TotalUsers = users.Count,
                VerifiedUsers = users.Count(u => u.IsVerified),
                BlockedUsers = users.Count(u => u.IsBlocked),
            };

            return Result<AdminUsersDto>.Success(vm);
        }
    }

}
