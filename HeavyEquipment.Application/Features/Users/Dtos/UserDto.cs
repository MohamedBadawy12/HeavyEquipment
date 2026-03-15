namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public record UserDto(
        Guid Id,
        string FullName,
        string Email,
        string Phone,
        string Role,
        decimal TrustScore,
        bool IsVerified,
        bool IsBlocked,
        DateTime CreatedAt
    );
}
