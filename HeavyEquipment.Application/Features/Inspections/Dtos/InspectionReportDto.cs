namespace HeavyEquipment.Application.Features.Inspections.Dtos
{
    public record InspectionReportDto(
        Guid Id,
        Guid RentalOrderId,
        string Type,
        string Result,
        string Notes,
        int HoursReading,
        DateTime InspectionDate,
        Guid InspectorId,
        IReadOnlyCollection<string> PhotoUrls
    );
}
