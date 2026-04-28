using System.Collections;
using EVWarranty.Domain.Entities;
using EVWarranty.Domain.Entities.Interfaces;
using EVWarranty.Infrastructure.Data;

namespace EVWarranty.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvWarrantyDbContext _context;
    private Hashtable? _repositories;

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
    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories == null) _repositories = new Hashtable();
        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }
        return (IGenericRepository<T>)_repositories[type]!;
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
