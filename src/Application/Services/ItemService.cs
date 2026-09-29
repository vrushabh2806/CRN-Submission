using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;

namespace ProductsAPI.Application.Services;

/// <summary>
/// Service implementing business logic for Item CRUD operations.
/// </summary>
public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<ItemDto>> GetItemsByProductAsync(int productId, int page, int pageSize, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching items for ProductId={ProductId}", productId);
        if (!await _unitOfWork.Products.ExistsAsync(productId, ct))
            throw new NotFoundException(nameof(Product), productId);

        var paged = await _unitOfWork.Items.GetByProductIdAsync(productId, page, pageSize, ct);
        return new PagedResult<ItemDto>
        {
            Items = _mapper.Map<List<ItemDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<ItemDto> GetItemByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);
        return _mapper.Map<ItemDto>(item);
    }

    /// <inheritdoc/>
    public async Task<ItemDto> CreateItemAsync(CreateItemDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating item for ProductId={ProductId}", dto.ProductId);
        if (!await _unitOfWork.Products.ExistsAsync(dto.ProductId, ct))
            throw new NotFoundException(nameof(Product), dto.ProductId);

        var item = _mapper.Map<Item>(dto);
        var created = await _unitOfWork.Items.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Item created with Id={Id}", created.Id);
        return _mapper.Map<ItemDto>(created);
    }

    /// <inheritdoc/>
    public async Task<ItemDto> UpdateItemAsync(int id, UpdateItemDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating item Id={Id}", id);
        var item = await _unitOfWork.Items.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);
        _mapper.Map(dto, item);
        await _unitOfWork.Items.UpdateAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<ItemDto>(item);
    }

    /// <inheritdoc/>
    public async Task DeleteItemAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting item Id={Id}", id);
        var item = await _unitOfWork.Items.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);
        await _unitOfWork.Items.DeleteAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
