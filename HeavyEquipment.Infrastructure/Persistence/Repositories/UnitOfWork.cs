using HeavyEquipment.Domain.Interfaces;
using HeavyEquipment.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HeavyEquipment.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IEquipmentRepository? _equipments;
        private IRentalOrderRepository? _rentalOrders;
        private IInspectionReportRepository? _inspectionReports;
        private IInsurancePolicyRepository? _insurancePolicies;
        private IReviewRepository? _reviews;
        private INotificationRepository? _notifications;
        private ILogisticsProviderRepository? _logisticsProviders;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IEquipmentRepository Equipments
            => _equipments ??= new EquipmentRepository(_context);

        public IRentalOrderRepository RentalOrders
            => _rentalOrders ??= new RentalOrderRepository(_context);

        public IInspectionReportRepository InspectionReports
            => _inspectionReports ??= new InspectionReportRepository(_context);

        public IInsurancePolicyRepository InsurancePolicies
            => _insurancePolicies ??= new InsurancePolicyRepository(_context);

        public IReviewRepository Reviews
            => _reviews ??= new ReviewRepository(_context);

        public INotificationRepository Notifications
            => _notifications ??= new NotificationRepository(_context);

        public ILogisticsProviderRepository LogisticsProviders
            => _logisticsProviders ??= new LogisticsProviderRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
           => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null)
                throw new InvalidOperationException("لم يتم بدء Transaction");

            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;

            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
