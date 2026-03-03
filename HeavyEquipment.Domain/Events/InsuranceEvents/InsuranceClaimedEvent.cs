namespace HeavyEquipment.Domain.Events.InsuranceEvents
{
    public class InsuranceClaimedEvent : BaseDomainEvent
    {
        public Guid InsurancePolicyId { get; }
        public Guid RentalOrderId { get; }
        public string Reason { get; }

        public InsuranceClaimedEvent(Guid insurancePolicyId, Guid rentalOrderId, string reason)
        {
            InsurancePolicyId = insurancePolicyId;
            RentalOrderId = rentalOrderId;
            Reason = reason;
        }
    }
}
