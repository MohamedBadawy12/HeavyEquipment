namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public class AdminUserRow
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public decimal TrustScore { get; set; }
        public bool IsVerified { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
