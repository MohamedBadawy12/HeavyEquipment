using HeavyEquipment.Domain.Interfaces;

namespace HeavyEquipment.Domain.Events
{
    public abstract class BaseDomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
