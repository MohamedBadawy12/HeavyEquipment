namespace HeavyEquipment.Domain.Events.RentalEvents
{
    public class RentalCompletedEvent : BaseDomainEvent
    {
        public Guid RentalOrderId { get; }
        public Guid CustomerId { get; }
        public Guid EquipmentId { get; }
        public decimal TotalPrice { get; }

        public RentalCompletedEvent(Guid rentalOrderId, Guid customerId, Guid equipmentId, decimal totalPrice)
        {
            RentalOrderId = rentalOrderId;
            CustomerId = customerId;
            EquipmentId = equipmentId;
            TotalPrice = totalPrice;
        }
    }
}
