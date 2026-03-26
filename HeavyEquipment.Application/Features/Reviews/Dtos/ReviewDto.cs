namespace HeavyEquipment.Application.Features.Reviews.Dtos
{
    public record ReviewDto(
        Guid Id,
        Guid RentalOrderId,
        Guid ReviewerId,
        string ReviewerName,
        int Rating,
        string Comment,
        string Type,
        DateTime CreatedAt);
}
