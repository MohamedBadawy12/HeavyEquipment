namespace HeavyEquipment.Domain.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(string toPhone, string message);
    }
}
