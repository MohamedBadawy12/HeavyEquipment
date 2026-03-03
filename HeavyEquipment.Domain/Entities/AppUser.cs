using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string NationalId { get; private set; }
        public UserType Role { get; private set; } // Owner or Customer or Admin
        public decimal TrustScore { get; private set; } = 100;
        public bool IsVerified { get; private set; } = false;
        public bool IsBlocked { get; private set; } = false;

        private readonly List<Equipment> _ownedEquipments = new();
        public virtual IReadOnlyCollection<Equipment> OwnedEquipments => _ownedEquipments.AsReadOnly();

        private readonly List<RentalOrder> _myRentals = new();
        public virtual IReadOnlyCollection<RentalOrder> MyRentals => _myRentals.AsReadOnly();

        private readonly List<Notification> _notifications = new();
        public virtual IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

        protected AppUser() { }
        public AppUser(string fullName, string email, string phoneNumber, string nationalId, UserType role)
        {
            if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("الاسم مطلوب");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("البريد الإلكتروني مطلوب");
            if (string.IsNullOrWhiteSpace(phoneNumber)) throw new DomainException("رقم الهاتف مطلوب");

            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            NationalId = nationalId;
            Role = role;
        }

        public void Verify()
        {
            if (IsVerified) throw new DomainException("المستخدم تم التحقق منه مسبقاً");
            IsVerified = true;
            SetUpdatedAt();
        }

        public void Block()
        {
            IsBlocked = true;
            SetUpdatedAt();
        }

        public void Unblock()
        {
            IsBlocked = false;
            SetUpdatedAt();
        }

        public void AdjustTrustScore(decimal delta)
        {
            TrustScore = Math.Clamp(TrustScore + delta, 0, 100);
            SetUpdatedAt();
        }

        public void AddNotification(Notification notification)
            => _notifications.Add(notification);
    }
}
