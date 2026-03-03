namespace HeavyEquipment.Domain.Events.RentalEvents
{
    public class RentalConfirmedEvent : BaseDomainEvent
    {
        public Guid RentalOrderId { get; }
        public Guid CustomerId { get; }
        public Guid EquipmentId { get; }

        public RentalConfirmedEvent(Guid rentalOrderId, Guid customerId, Guid equipmentId)
        {
            RentalOrderId = rentalOrderId;
            CustomerId = customerId;
            EquipmentId = equipmentId;
        }
    }
}
