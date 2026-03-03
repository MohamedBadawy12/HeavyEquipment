using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    /// <summary>
    /// تقرير الفحص
    /// </summary>
    public class InspectionReport : BaseEntity
    {
        public Guid RentalOrderId { get; private set; }
        public virtual RentalOrder RentalOrder { get; private set; }
        public InspectionType Type { get; private set; }
        public InspectionStatus Result { get; private set; }
        public string Notes { get; private set; }
        public int HoursReading { get; private set; }
        public DateTime InspectionDate { get; private set; }
        public Guid InspectorId { get; private set; }

        private readonly List<string> _photoUrls = new();
        public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();


        protected InspectionReport() { }
        public InspectionReport(
            Guid rentalOrderId,
            Guid inspectorId,
            InspectionType type,
            InspectionStatus result,
            int hoursReading,
            string notes = "")
        {
            if (hoursReading < 0) throw new DomainException("قراءة الساعات لا يمكن أن تكون سالبة");

            RentalOrderId = rentalOrderId;
            InspectorId = inspectorId;
            Type = type;
            Result = result;
            HoursReading = hoursReading;
            Notes = notes;
            InspectionDate = DateTime.UtcNow;
        }


        public void AddPhoto(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("رابط الصورة لا يمكن أن يكون فارغاً");

            _photoUrls.Add(url);
        }

        public void UpdateResult(InspectionStatus newResult, string notes)
        {
            Result = newResult;
            Notes = notes;
            SetUpdatedAt();
        }
    }
}
