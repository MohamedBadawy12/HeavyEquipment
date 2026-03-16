using MediatR;

namespace HeavyEquipment.Domain.Interfaces
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }
}
