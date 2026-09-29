using ProductsAPI.Application.DTOs;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Application.Interfaces;

/// <summary>Repository interface for Product operations.</summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product?> GetByIdWithItemsAsync(int id, CancellationToken ct = default);
    Task<PagedResult<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Product product, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}

/// <summary>Repository interface for Item operations.</summary>
public interface IItemRepository
{
    Task<Item?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<Item>> GetByProductIdAsync(int productId, int page, int pageSize, CancellationToken ct = default);
    Task<Item> AddAsync(Item item, CancellationToken ct = default);
    Task UpdateAsync(Item item, CancellationToken ct = default);
    Task DeleteAsync(Item item, CancellationToken ct = default);
}
