using Microsoft.EntityFrameworkCore;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data.Repositories;

/// <summary>EF Core implementation of IProductRepository.</summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Products.FindAsync(new object[] { id }, ct);

    public async Task<Product?> GetByIdWithItemsAsync(int id, CancellationToken ct = default)
        => await _context.Products
            .AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PagedResult<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Products.AsNoTracking().OrderBy(p => p.Id);
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
        return product;
    }

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await _context.Products.AnyAsync(p => p.Id == id, ct);
}
