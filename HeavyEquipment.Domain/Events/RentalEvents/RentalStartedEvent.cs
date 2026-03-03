namespace HeavyEquipment.Domain.Events.RentalEvents
{
    public class RentalStartedEvent : BaseDomainEvent
    {
        public Guid RentalOrderId { get; }
        public Guid EquipmentId { get; }

        public RentalStartedEvent(Guid rentalOrderId, Guid equipmentId)
        {
            RentalOrderId = rentalOrderId;
            EquipmentId = equipmentId;
        }
    }
}
