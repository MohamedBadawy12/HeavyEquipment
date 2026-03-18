namespace HeavyEquipment.Application.Features.Logistics.Dtos
{
    public record LogisticsProviderDto(
        Guid Id,
        string CompanyName,
        string ContactNumber,
        decimal RatePerKilometer,
        bool IsActive
    );
}
