namespace HeavyEquipment.Application.Features.Equipments.Dto
{
    public record EquipmentDto(
       Guid Id,
       string Name,
       string Description,
       string Model,
       int ManufactureYear,
       string Category,
       string Status,
       decimal HourlyRate,
       decimal DepositAmount,
       string City,
       string Address,
       string OwnerName,
       IReadOnlyCollection<string> PhotoUrls
   );

}
