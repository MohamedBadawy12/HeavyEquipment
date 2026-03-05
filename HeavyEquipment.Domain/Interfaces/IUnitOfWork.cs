namespace HeavyEquipment.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEquipmentRepository Equipments { get; }
        IRentalOrderRepository RentalOrders { get; }
        IInspectionReportRepository InspectionReports { get; }
        IInsurancePolicyRepository InsurancePolicies { get; }
        IReviewRepository Reviews { get; }
        INotificationRepository Notifications { get; }
        ILogisticsProviderRepository LogisticsProviders { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
