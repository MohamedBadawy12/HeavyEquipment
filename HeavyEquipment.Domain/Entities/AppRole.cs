using Microsoft.AspNetCore.Identity;

namespace HeavyEquipment.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public AppRole() { }

        public AppRole(string roleName, string? description = null)
            : base(roleName)
        {
            Id = Guid.NewGuid();
            Description = description;
        }
    }
}
