namespace HeavyEquipment.Domain.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default);
    }
}
