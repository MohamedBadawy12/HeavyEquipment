using HeavyEquipment.Application.Common.Models;
using HeavyEquipment.Application.Features.Equipments.Commands.Queries;
using HeavyEquipment.Application.Features.RentalOrders.Commands.Queries;
using HeavyEquipment.Application.Features.Users.Dtos;
using HeavyEquipment.Domain.Entities;
using HeavyEquipment.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Application.Features.Users.Commands.Queries
{
    public record GetAdminDashboardQuery() : IRequest<Result<AdminDashboardDto>>;

    public class GetAdminDashboardHandler
       : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;

        public GetAdminDashboardHandler(IMediator mediator, UserManager<AppUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<Result<AdminDashboardDto>> Handle(
            GetAdminDashboardQuery request, CancellationToken ct)
        {
            var users = await _userManager.Users
                                .Where(u => !u.IsDeleted)
                                .ToListAsync(ct);

            var pagedEquipments = await _mediator.Send(
            new GetAvailableEquipmentsQuery(DateTime.UtcNow, DateTime.UtcNow.AddYears(1)), ct);
            var equipments = pagedEquipments?.Items?.ToList() ?? new();

            var orders = (await _mediator.Send(new GetActiveOrdersQuery(), ct))?.ToList() ?? new();

            var now = DateTime.UtcNow;

            var vm = new AdminDashboardDto
            {
                TotalUsers = users.Count,
                TotalOwners = users.Count(u => u.Role == UserType.Owner),
                TotalCustomers = users.Count(u => u.Role == UserType.Customer),
                VerifiedUsers = users.Count(u => u.IsVerified),
                BlockedUsers = users.Count(u => u.IsBlocked),

                TotalEquipments = equipments.Count,
                AvailableEquipments = equipments.Count(e => e.Status == EquipmentStatus.Available.ToString()),
                RentedEquipments = equipments.Count(e => e.Status == EquipmentStatus.Rented.ToString()),

                TotalOrders = orders.Count,
                ActiveOrders = orders.Count(o => o.Status == OrderStatus.Active.ToString()),
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending.ToString()),
                CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed.ToString()),

                TotalRevenue = orders
                    .Where(o => o.Status == OrderStatus.Completed.ToString())
                    .Sum(o => o.TotalPrice),

                MonthRevenue = orders
                    .Where(o => o.Status == OrderStatus.Completed.ToString()
                             && o.RentalEnd.Month == now.Month
                             && o.RentalEnd.Year == now.Year)
                    .Sum(o => o.TotalPrice),

                RecentUsers = users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new AdminUserRow
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
                    }).ToList(),

                RecentOrders = orders
                    .OrderByDescending(o => o.RentalStart)
                    .Take(5)
                    .ToList()
            };

            return Result<AdminDashboardDto>.Success(vm);
        }
    }
}
