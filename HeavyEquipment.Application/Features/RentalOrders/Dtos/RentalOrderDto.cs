namespace HeavyEquipment.Application.Features.RentalOrders.Dtos
{
    public record RentalOrderDto(
       Guid Id,
       string EquipmentName,
       string CustomerName,
       DateTime RentalStart,
       DateTime RentalEnd,
       decimal TotalPrice,
       string Status,
       bool HasInsurance,
       string? LogisticsProviderName,
       int InspectionsCount,
       int ReviewsCount
   );
}
