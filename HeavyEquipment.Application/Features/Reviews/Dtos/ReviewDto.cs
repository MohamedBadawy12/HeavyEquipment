namespace HeavyEquipment.Application.Features.Reviews.Dtos
{
    public record ReviewDto(
        Guid Id,
        int Rating,
        string Comment,
        string ReviewerName,
        string Type,
        DateTime CreatedAt);
}
