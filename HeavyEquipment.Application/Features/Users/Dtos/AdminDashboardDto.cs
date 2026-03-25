using HeavyEquipment.Application.Features.RentalOrders.Dtos;

namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public class AdminDashboardDto
    {
        // Users
        public int TotalUsers { get; set; }
        public int TotalOwners { get; set; }
        public int TotalCustomers { get; set; }
        public int VerifiedUsers { get; set; }
        public int BlockedUsers { get; set; }

        // Equipments
        public int TotalEquipments { get; set; }
        public int AvailableEquipments { get; set; }
        public int RentedEquipments { get; set; }

        // Orders
        public int TotalOrders { get; set; }
        public int ActiveOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }

        // Revenue
        public decimal TotalRevenue { get; set; }
        public decimal MonthRevenue { get; set; }

        // Recent
        public List<AdminUserRow> RecentUsers { get; set; } = new();
        public List<RentalOrderSummaryDto> RecentOrders { get; set; } = new();

        // نمو المستخدمين آخر 6 شهور
        public List<string> UserGrowthLabels { get; set; } = new();
        public List<int> UserGrowthValues { get; set; } = new();

        // توزيع المستخدمين (Owner vs Customer)
        public List<int> UserRoleDistribution { get; set; } = new();
        // حالات المعدات الكلية في السيستم
        public List<int> EquipmentStatusStats { get; set; } = new();
    }
}
