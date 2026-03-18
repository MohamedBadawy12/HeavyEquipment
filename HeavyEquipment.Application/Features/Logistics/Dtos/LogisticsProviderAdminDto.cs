namespace HeavyEquipment.Application.Features.Logistics.Dtos
{
    public record LogisticsProviderAdminDto(
        Guid Id,
        string CompanyName,
        string ContactNumber,
        decimal RatePerKilometer,
        bool IsActive
    );
}
