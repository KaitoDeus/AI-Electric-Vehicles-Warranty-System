using EVWarranty.Domain.Entities;

namespace EVWarranty.Domain.Entities.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Vehicle> Vehicles { get; }
    IGenericRepository<WarrantyClaim> Claims { get; }
    IGenericRepository<User> Users { get; }
    Task<int> CompleteAsync(); // Lưu tất cả thay đổi vào DB
}
