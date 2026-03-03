using HeavyEquipment.Domain.Enums;
using HeavyEquipment.Domain.Exceptions;

namespace HeavyEquipment.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid RentalOrderId { get; private set; }
        public virtual RentalOrder RentalOrder { get; private set; }
        public Guid ReviewerId { get; private set; }
        public virtual AppUser Reviewer { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public ReviewType Type { get; private set; }

        protected Review() { }

        public Review(Guid rentalOrderId, Guid reviewerId, int rating, string comment, ReviewType type)
        {
            if (rating < 1 || rating > 5)
                throw new DomainException("التقييم يجب أن يكون بين 1 و 5");
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("التعليق مطلوب");

            RentalOrderId = rentalOrderId;
            ReviewerId = reviewerId;
            Rating = rating;
            Comment = comment;
            Type = type;
        }

        public void EditComment(string newComment)
        {
            if (string.IsNullOrWhiteSpace(newComment))
                throw new DomainException("التعليق لا يمكن أن يكون فارغاً");

            Comment = newComment;
            SetUpdatedAt();
        }
    }
}
