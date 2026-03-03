using HeavyEquipment.Domain.Events.InsuranceEvents;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    /// <summary>
    /// وثيقة التأمين
    /// </summary>
    public class InsurancePolicy : BaseEntity
    {
        public Guid RentalOrderId { get; private set; }
        public virtual RentalOrder RentalOrder { get; private set; }
        public string PolicyNumber { get; private set; }
        public decimal CoverageAmount { get; private set; }
        public decimal PremiumAmount { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsClaimed { get; private set; } = false;
        public string? ClaimReason { get; private set; }
        public DateTime? ClaimedAt { get; private set; }


        protected InsurancePolicy() { }
        public InsurancePolicy(Guid rentalOrderId, decimal coverageAmount, decimal premiumAmount, DateTime expiryDate)
        {
            if (coverageAmount <= 0) throw new DomainException("مبلغ التغطية يجب أن يكون أكبر من صفر");
            if (premiumAmount <= 0) throw new DomainException("قيمة القسط يجب أن تكون أكبر من صفر");
            if (expiryDate <= DateTime.UtcNow) throw new DomainException("تاريخ انتهاء الوثيقة غير صحيح");

            RentalOrderId = rentalOrderId;
            PolicyNumber = $"INS-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            CoverageAmount = coverageAmount;
            PremiumAmount = premiumAmount;
            ExpiryDate = expiryDate;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiryDate;

        public void SubmitClaim(string reason)
        {
            if (IsClaimed)
                throw new DomainException("تم تقديم مطالبة تأمينية مسبقاً");
            if (IsExpired())
                throw new DomainException("انتهت صلاحية وثيقة التأمين");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("سبب المطالبة مطلوب");

            IsClaimed = true;
            ClaimReason = reason;
            ClaimedAt = DateTime.UtcNow;
            SetUpdatedAt();

            AddDomainEvent(new InsuranceClaimedEvent(Id, RentalOrderId, reason));
        }
    }
}
