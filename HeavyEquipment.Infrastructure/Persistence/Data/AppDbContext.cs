using HeavyEquipment.Domain;
using HeavyEquipment.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeavyEquipment.Infrastructure.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Equipment> Equipments => Set<Equipment>();
        public DbSet<RentalOrder> RentalOrders => Set<RentalOrder>();
        public DbSet<InspectionReport> InspectionReports => Set<InspectionReport>();
        public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
        public DbSet<LogisticsProvider> LogisticsProviders => Set<LogisticsProvider>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<AppRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (_mediator is not null)
            {
                foreach (var domainEvent in domainEvents)
                    await _mediator.Publish(domainEvent, cancellationToken);
            }

            ChangeTracker
                .Entries<BaseEntity>()
                .ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            return result;
        }
    }
}
