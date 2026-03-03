namespace HeavyEquipment.Domain.Events.RentalEvents
{
    public class RentalDisputedEvent : BaseDomainEvent
    {
        public Guid RentalOrderId { get; }
        public string Reason { get; }

        public RentalDisputedEvent(Guid rentalOrderId, string reason)
        {
            RentalOrderId = rentalOrderId;
            Reason = reason;
        }
    }
}
