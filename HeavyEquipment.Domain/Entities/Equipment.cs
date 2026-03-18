using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Events.EquipmentEvents;
using HeavyEquipment.Domain.Exceptions;
using HeavyEquipment.Domain.ValueObjects;

namespace HeavyEquipment.Domain.Entities
{
    /// <summary>
    /// المعدات 
    /// </summary>
    public class Equipment : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Model { get; private set; }
        public int ManufactureYear { get; private set; }
        public EquipmentCategory Category { get; private set; }
        public EquipmentStatus Status { get; private set; }
        public decimal HourlyRate { get; private set; }
        public decimal DepositAmount { get; private set; }
        public int TotalHoursOperated { get; private set; }
        public int NextMaintenanceThreshold { get; private set; }
        public Location CurrentLocation { get; private set; }
        public Guid OwnerId { get; private set; }
        public virtual AppUser Owner { get; private set; }

        private readonly List<RentalOrder> _rentals = new();
        public virtual IReadOnlyCollection<RentalOrder> Rentals => _rentals.AsReadOnly();

        private readonly List<string> _photoUrls = new();
        public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

        protected Equipment() { }

        public Equipment(
            string name,
            string description,
            string model,
            int manufactureYear,
            EquipmentCategory category,
            decimal hourlyRate,
            decimal depositAmount,
            int maintenanceThreshold,
            Guid ownerId,
            Location location)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("اسم المعدة مطلوب");
            if (hourlyRate <= 0) throw new DomainException("السعر يجب أن يكون أكبر من صفر");
            if (manufactureYear < 1900 || manufactureYear > DateTime.UtcNow.Year)
                throw new DomainException("سنة الصنع غير صحيحة");

            Name = name;
            Description = description;
            Model = model;
            ManufactureYear = manufactureYear;
            Category = category;
            HourlyRate = hourlyRate;
            DepositAmount = depositAmount;
            NextMaintenanceThreshold = maintenanceThreshold;
            OwnerId = ownerId;
            CurrentLocation = location;
            Status = EquipmentStatus.Available;
        }
        public bool IsReadyForRental()
        {
            int remainingHours = NextMaintenanceThreshold - TotalHoursOperated;
            return Status == EquipmentStatus.Available && remainingHours > 10;
        }

        public void UpdateStatus(EquipmentStatus newStatus)
        {
            Status = newStatus;
            SetUpdatedAt();
        }

        public void UpdateLocation(Location newLocation)
        {
            CurrentLocation = newLocation ?? throw new DomainException("الموقع مطلوب");
            SetUpdatedAt();
        }

        public void UpdateHourlyRate(decimal newRate)
        {
            if (newRate <= 0) throw new DomainException("السعر يجب أن يكون أكبر من صفر");
            HourlyRate = newRate;
            SetUpdatedAt();
        }

        // إضافة طلب تأجير للمعدة 
        public void AddRental(RentalOrder order)
        {
            if (!IsReadyForRental())
                throw new DomainException("المعدة غير متاحة للتأجير حالياً");

            _rentals.Add(order);
            UpdateStatus(EquipmentStatus.Rented);
            SetUpdatedAt();
        }


        // تسجيل ساعات التشغيل بعد انتهاء كل تأجير
        public void LogOperatingHours(int hours)
        {
            if (hours <= 0) throw new DomainException("عدد الساعات يجب أن يكون أكبر من صفر");

            TotalHoursOperated += hours;
            SetUpdatedAt();

            int remainingHours = NextMaintenanceThreshold - TotalHoursOperated;

            if (remainingHours <= 10)
            {
                AddDomainEvent(new MaintenanceAlertEvent(Id, OwnerId, remainingHours));
                UpdateStatus(EquipmentStatus.UnderMaintenance);
            }
        }

        public void AddPhoto(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new DomainException("رابط الصورة مطلوب");
            _photoUrls.Add(url);
        }

        public void CompleteMaintenanceAndReset(int newThreshold)
        {
            NextMaintenanceThreshold = TotalHoursOperated + newThreshold;
            UpdateStatus(EquipmentStatus.Available);
            SetUpdatedAt();
        }
    }
}
