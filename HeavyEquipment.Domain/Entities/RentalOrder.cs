using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.RentalEvents;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    /// <summary>
    /// طلب التأجير
    /// </summary>
    public class RentalOrder : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public virtual AppUser Customer { get; private set; }

        public Guid EquipmentId { get; private set; }
        public virtual Equipment Equipment { get; private set; }

        public DateTime RentalStart { get; private set; }
        public DateTime RentalEnd { get; private set; }
        public decimal HourlyRate { get; private set; }
        public decimal TotalPrice { get; private set; }
        public OrderStatus Status { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? DisputeReason { get; private set; }

        public Guid? LogisticsProviderId { get; private set; }
        public virtual LogisticsProvider? LogisticsProvider { get; private set; }

        public virtual InsurancePolicy? Insurance { get; private set; }

        private readonly List<InspectionReport> _inspections = new();
        public virtual IReadOnlyCollection<InspectionReport> Inspections => _inspections.AsReadOnly();

        private readonly List<Review> _reviews = new();
        public virtual IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

        protected RentalOrder() { }
        public RentalOrder(Guid customerId, Guid equipmentId, DateTime start, DateTime end, decimal hourlyRate)
        {
            if (end <= start)
                throw new DomainException("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء");
            if (start < DateTime.UtcNow.Date)
                throw new DomainException("لا يمكن الحجز في تاريخ ماضٍ");
            if (hourlyRate <= 0)
                throw new DomainException("السعر غير صحيح");

            CustomerId = customerId;
            EquipmentId = equipmentId;
            RentalStart = start;
            RentalEnd = end;
            HourlyRate = hourlyRate;

            var hours = (decimal)(end - start).TotalHours;
            TotalPrice = Math.Round(hours * hourlyRate, 2);

            Status = OrderStatus.Pending;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("فقط الطلبات المعلقة يمكن تأكيدها");

            Status = OrderStatus.Confirmed;
            SetUpdatedAt();
            AddDomainEvent(new RentalConfirmedEvent(Id, CustomerId, EquipmentId));
        }

        public void MarkAsActive()
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("يجب تأكيد الطلب قبل البدء");

            Status = OrderStatus.Active;
            SetUpdatedAt();
            AddDomainEvent(new RentalStartedEvent(Id, EquipmentId));
        }

        public void Complete()
        {
            if (Status != OrderStatus.Active)
                throw new DomainException("لا يمكن إتمام طلب غير نشط");

            Status = OrderStatus.Completed;
            SetUpdatedAt();
            AddDomainEvent(new RentalCompletedEvent(Id, CustomerId, EquipmentId, TotalPrice));
        }

        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Completed)
                throw new DomainException("لا يمكن إلغاء طلب مكتمل");
            if (Status == OrderStatus.Cancelled)
                throw new DomainException("الطلب ملغي مسبقاً");

            CancellationReason = reason;
            Status = OrderStatus.Cancelled;
            SetUpdatedAt();
            AddDomainEvent(new RentalCancelledEvent(Id, CustomerId, reason));
        }

        public void RaiseDispute(string reason)
        {
            if (Status != OrderStatus.Active && Status != OrderStatus.Completed)
                throw new DomainException("النزاع يرفع فقط على الطلبات النشطة أو المكتملة");

            DisputeReason = reason;
            Status = OrderStatus.Disputed;
            SetUpdatedAt();
            AddDomainEvent(new RentalDisputedEvent(Id, reason));
        }

        public void SetInsurance(InsurancePolicy policy)
        {
            Insurance = policy ?? throw new DomainException("بيانات التأمين غير صحيحة");
            SetUpdatedAt();
        }

        public void AssignLogistics(LogisticsProvider provider)
        {
            LogisticsProviderId = provider.Id;
            SetUpdatedAt();
        }

        public void AddInspection(InspectionReport report)
        {
            _inspections.Add(report);
            SetUpdatedAt();
        }

        public void AddReview(Review review)
        {
            if (Status != OrderStatus.Completed)
                throw new DomainException("يمكن التقييم فقط بعد اكتمال الطلب");

            bool reviewerAlreadyReviewed = _reviews.Any(r => r.ReviewerId == review.ReviewerId);
            if (reviewerAlreadyReviewed)
                throw new DomainException("قمت بالتقييم مسبقاً");

            _reviews.Add(review);
            SetUpdatedAt();
        }

        public int GetRentalDurationInHours()
            => (int)(RentalEnd - RentalStart).TotalHours;

        public bool IsOverdue()
            => Status == OrderStatus.Active && DateTime.UtcNow > RentalEnd;
    }
}
