namespace HeavyEquipment.Domain.Events.EquipmentEvents
{
    public class MaintenanceAlertEvent : BaseDomainEvent
    {
        public Guid EquipmentId { get; }
        public Guid OwnerId { get; }
        public int RemainingHours { get; }

        public MaintenanceAlertEvent(Guid equipmentId, Guid ownerId, int remainingHours)
        {
            EquipmentId = equipmentId;
            OwnerId = ownerId;
            RemainingHours = remainingHours;
        }
    }
}
