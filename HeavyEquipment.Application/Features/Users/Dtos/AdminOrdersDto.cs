using HeavyEquipment.Application.Features.RentalOrders.Dtos;

namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public class AdminOrdersDto
    {
        public List<RentalOrderSummaryDto> Orders { get; set; } = new();
        public string? StatusFilter { get; set; }
        public int TotalOrders { get; set; }
        public int ActiveOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
