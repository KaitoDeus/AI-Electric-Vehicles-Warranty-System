using EVWarranty.Domain.Entities;

namespace EVWarranty.Domain.Entities.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Vehicle> Vehicles { get; }
    IGenericRepository<WarrantyClaim> Claims { get; }
    IGenericRepository<User> Users { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
}
