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
    double TotalHoursOperated,
    double NextMaintenanceThreshold,
    IReadOnlyList<string> PhotoUrls,
    string City,
    string Address,
    double Latitude,
    double Longitude,
    Guid OwnerId,
    string OwnerName,
    decimal OwnerTrustScore,
    bool OwnerIsVerified
    );

}
