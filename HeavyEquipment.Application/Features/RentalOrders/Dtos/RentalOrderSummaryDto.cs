namespace HeavyEquipment.Application.Features.RentalOrders.Dtos
{
    public record RentalOrderSummaryDto(
         Guid Id,
         string EquipmentName,
         DateTime RentalStart,
         DateTime RentalEnd,
         decimal TotalPrice,
         string Status
     );
}
