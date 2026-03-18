namespace HeavyEquipment.Application.Features.Insurances.Dtos
{
    public record InsurancePolicyDto(
        Guid Id,
        Guid RentalOrderId,
        string PolicyNumber,
        decimal CoverageAmount,
        decimal PremiumAmount,
        DateTime ExpiryDate,
        bool IsClaimed,
        string? ClaimReason,
        DateTime? ClaimedAt
    );
}
