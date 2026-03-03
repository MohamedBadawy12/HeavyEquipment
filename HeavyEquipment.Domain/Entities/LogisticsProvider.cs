using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    /// <summary>
    /// شركة النقل
    /// </summary>
    public class LogisticsProvider : BaseEntity
    {
        public string CompanyName { get; private set; }
        public string ContactNumber { get; private set; }
        public decimal RatePerKilometer { get; private set; }
        public bool IsActive { get; private set; } = true;

        protected LogisticsProvider() { }
        public LogisticsProvider(string companyName, string contactNumber, decimal ratePerKm)
        {
            if (string.IsNullOrWhiteSpace(companyName)) throw new DomainException("اسم الشركة مطلوب");
            if (string.IsNullOrWhiteSpace(contactNumber)) throw new DomainException("رقم التواصل مطلوب");
            if (ratePerKm <= 0) throw new DomainException("سعر الكيلومتر يجب أن يكون أكبر من صفر");

            CompanyName = companyName;
            ContactNumber = contactNumber;
            RatePerKilometer = ratePerKm;
        }

        public decimal CalculateShippingCost(double distanceInKm)
        {
            if (distanceInKm <= 0) throw new DomainException("المسافة يجب أن تكون أكبر من صفر");
            return Math.Round((decimal)distanceInKm * RatePerKilometer, 2);
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }

        public void UpdateRate(decimal newRate)
        {
            if (newRate <= 0) throw new DomainException("السعر الجديد يجب أن يكون أكبر من صفر");
            RatePerKilometer = newRate;
            SetUpdatedAt();
        }

    }
}
