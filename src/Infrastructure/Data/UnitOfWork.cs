using Microsoft.EntityFrameworkCore.Storage;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Infrastructure.Data.Repositories;

namespace ProductsAPI.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation that wraps all repositories under a single DbContext transaction.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IProductRepository? _products;
    private IItemRepository? _items;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public IItemRepository Items =>
        _items ??= new ItemRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
