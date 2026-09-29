using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;

namespace ProductsAPI.Application.Services;

/// <summary>
/// Service implementing business logic for Product CRUD operations.
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<ProductDto>> GetAllProductsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all products. Page={Page}, PageSize={PageSize}", page, pageSize);
        var pagedProducts = await _unitOfWork.Products.GetAllAsync(page, pageSize, ct);
        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(pagedProducts.Items),
            TotalCount = pagedProducts.TotalCount,
            Page = pagedProducts.Page,
            PageSize = pagedProducts.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<ProductDto> GetProductByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching product with Id={Id}", id);
        var product = await _unitOfWork.Products.GetByIdWithItemsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);
        return _mapper.Map<ProductDto>(product);
    }

    /// <inheritdoc/>
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, string createdBy, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating product: {ProductName} by {CreatedBy}", dto.ProductName, createdBy);
        var product = _mapper.Map<Product>(dto);
        product.CreatedBy = createdBy;
        product.CreatedOn = DateTime.UtcNow;
        var created = await _unitOfWork.Products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Product created with Id={Id}", created.Id);
        return _mapper.Map<ProductDto>(created);
    }

    /// <inheritdoc/>
    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto, string modifiedBy, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating product Id={Id}", id);
        var product = await _unitOfWork.Products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);
        _mapper.Map(dto, product);
        product.ModifiedBy = modifiedBy;
        product.ModifiedOn = DateTime.UtcNow;
        await _unitOfWork.Products.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Product Id={Id} updated", id);
        return _mapper.Map<ProductDto>(product);
    }

    /// <inheritdoc/>
    public async Task DeleteProductAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting product Id={Id}", id);
        var product = await _unitOfWork.Products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);
        await _unitOfWork.Products.DeleteAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Product Id={Id} deleted", id);
    }
}
