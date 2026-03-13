namespace HeavyEquipment.Application.Features.RentalOrders.Dtos
{
    public record RentalOrderSummaryDto(
         Guid Id,
         Guid EquipmentId,
         string EquipmentName,
         DateTime RentalStart,
         DateTime RentalEnd,
         decimal TotalPrice,
         string Status
     );
}
