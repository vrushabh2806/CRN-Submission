using Microsoft.EntityFrameworkCore;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data.Repositories;

/// <summary>EF Core implementation of IItemRepository.</summary>
public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext context) => _context = context;

    public async Task<Item?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<PagedResult<Item>> GetByProductIdAsync(int productId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Items.AsNoTracking()
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.Id);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Item>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Item> AddAsync(Item item, CancellationToken ct = default)
    {
        await _context.Items.AddAsync(item, ct);
        return item;
    }

    public Task UpdateAsync(Item item, CancellationToken ct = default)
    {
        _context.Items.Update(item);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Item item, CancellationToken ct = default)
    {
        _context.Items.Remove(item);
        return Task.CompletedTask;
    }
}
