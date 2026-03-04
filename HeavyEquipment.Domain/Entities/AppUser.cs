using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; private set; }
        public string NationalId { get; private set; }
        public UserType Role { get; private set; } // Owner or Customer or Admin
        public decimal TrustScore { get; private set; } = 100;
        public bool IsVerified { get; private set; } = false;
        public bool IsBlocked { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

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
            if (string.IsNullOrWhiteSpace(nationalId)) throw new DomainException("رقم الهوية مطلوب");

            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            UserName = email;
            PhoneNumber = phoneNumber;
            NationalId = nationalId;
            Role = role;
        }

        public void Verify()
        {
            if (IsVerified) throw new DomainException("المستخدم تم التحقق منه مسبقاً");
            IsVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            if (IsBlocked) throw new DomainException("المستخدم محظور مسبقاً");
            IsBlocked = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unblock()
        {
            if (!IsBlocked) throw new DomainException("المستخدم غير محظور");
            IsBlocked = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AdjustTrustScore(decimal delta)
        {
            TrustScore = Math.Clamp(TrustScore + delta, 0, 100);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddNotification(Notification notification)
            => _notifications.Add(notification);
    }
}
