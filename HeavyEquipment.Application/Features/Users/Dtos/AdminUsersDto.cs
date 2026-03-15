namespace HeavyEquipment.Application.Features.Users.Dtos
{
    public class AdminUsersDto
    {
        public List<AdminUserRow> Users { get; set; } = new();
        public string? RoleFilter { get; set; }
        public string? Search { get; set; }
        public int TotalUsers { get; set; }
        public int VerifiedUsers { get; set; }
        public int BlockedUsers { get; set; }
    }
}
