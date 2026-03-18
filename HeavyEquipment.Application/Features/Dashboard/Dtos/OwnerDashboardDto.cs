using HeavyEquipment.Application.Features.Equipments.Dto;
using HeavyEquipment.Application.Features.RentalOrders.Dtos;

namespace HeavyEquipment.Application.Features.Dashboard.Dtos
{
    public class OwnerDashboardDto
    {
        public int TotalEquipments { get; set; }
        public int AvailableEquipments { get; set; }
        public int RentedEquipments { get; set; }
        public int UnderMaintenance { get; set; }

        public int ActiveRentals { get; set; }
        public int PendingRentals { get; set; }
        public int CompletedRentals { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal MonthRevenue { get; set; }

        public List<EquipmentSummaryDto> Equipments { get; set; } = new();
        public List<RentalOrderSummaryDto> RecentOrders { get; set; } = new();
        public List<RentalOrderSummaryDto> PendingOrders { get; set; } = new();
    }
}
