using EVWarranty.Domain.Entities;
using EVWarranty.Domain.Entities.Interfaces;
using EVWarranty.Infrastructure.Data;

namespace EVWarranty.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvWarrantyDbContext _context;

    // Khai báo các Repository cụ thể
    public IGenericRepository<Vehicle> Vehicles { get; private set; }
    public IGenericRepository<WarrantyClaim> Claims { get; private set; }
    public IGenericRepository<User> Users { get; private set; }

    public UnitOfWork(EvWarrantyDbContext context)
    {
        _context = context;
        Vehicles = new GenericRepository<Vehicle>(_context);
        Claims = new GenericRepository<WarrantyClaim>(_context);
        Users = new GenericRepository<User>(_context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
