using HeavyEquipment.Application.Features.Equipments.Dto;
using HeavyEquipment.Application.Features.RentalOrders.Dtos;

namespace HeavyEquipment.WebMVC.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalEquipments { get; set; }
        public int ActiveRentals { get; set; }
        public List<EquipmentSummaryDto> Equipments { get; set; } = new();
        public List<RentalOrderSummaryDto> RecentOrders { get; set; } = new();
    }
}
