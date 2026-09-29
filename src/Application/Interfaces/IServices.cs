using ProductsAPI.Application.DTOs;

namespace ProductsAPI.Application.Interfaces;

/// <summary>Service interface for Product CRUD operations.</summary>
public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<ProductDto> GetProductByIdAsync(int id, CancellationToken ct = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto, string createdBy, CancellationToken ct = default);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto, string modifiedBy, CancellationToken ct = default);
    Task DeleteProductAsync(int id, CancellationToken ct = default);
}

/// <summary>Service interface for Item CRUD operations.</summary>
public interface IItemService
{
    Task<PagedResult<ItemDto>> GetItemsByProductAsync(int productId, int page, int pageSize, CancellationToken ct = default);
    Task<ItemDto> GetItemByIdAsync(int id, CancellationToken ct = default);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto, CancellationToken ct = default);
    Task<ItemDto> UpdateItemAsync(int id, UpdateItemDto dto, CancellationToken ct = default);
    Task DeleteItemAsync(int id, CancellationToken ct = default);
}

/// <summary>Service interface for authentication and token management.</summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
}
