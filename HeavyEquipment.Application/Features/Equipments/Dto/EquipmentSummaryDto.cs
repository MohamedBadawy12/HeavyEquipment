namespace HeavyEquipment.Application.Features.Equipments.Dto
{
    public record EquipmentSummaryDto(
        Guid Id,
        string Name,
        string Category,
        decimal HourlyRate,
        string City,
        string Status
    );
}
