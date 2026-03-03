namespace HeavyEquipment.Domain.Events.RentalEvents
{
    public class RentalCancelledEvent : BaseDomainEvent
    {
        public Guid RentalOrderId { get; }
        public Guid CustomerId { get; }
        public string Reason { get; }

        public RentalCancelledEvent(Guid rentalOrderId, Guid customerId, string reason)
        {
            RentalOrderId = rentalOrderId;
            CustomerId = customerId;
            Reason = reason;
        }
    }
}
