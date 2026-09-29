using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;

namespace ProductsAPI.API.Controllers;

/// <summary>
/// RESTful controller for Item CRUD operations within a product.
/// Supports both product-scoped and direct item endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[Authorize]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService) => _itemService = itemService;

    /// <summary>Gets paginated items belonging to a specific product.</summary>
    [HttpGet("products/{productId:int}/items")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProduct(
        int productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        pageSize = Math.Min(pageSize, 100);
        var result = await _itemService.GetItemsByProductAsync(productId, page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ItemDto>>.Ok(result));
    }

    /// <summary>Gets a specific item by ID.</summary>
    [HttpGet("items/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var item = await _itemService.GetItemByIdAsync(id, ct);
        return Ok(ApiResponse<ItemDto>.Ok(item));
    }

    /// <summary>Creates a new item for a product.</summary>
    [HttpPost("items")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateItemDto dto, CancellationToken ct = default)
    {
        var item = await _itemService.CreateItemAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id },
            ApiResponse<ItemDto>.Ok(item, "Item created successfully."));
    }

    /// <summary>Updates an existing item's quantity.</summary>
    [HttpPut("items/{id:int}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemDto dto, CancellationToken ct = default)
    {
        var item = await _itemService.UpdateItemAsync(id, dto, ct);
        return Ok(ApiResponse<ItemDto>.Ok(item, "Item updated successfully."));
    }

    /// <summary>Deletes an item by ID (Admin only).</summary>
    [HttpDelete("items/{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        await _itemService.DeleteItemAsync(id, ct);
        return NoContent();
    }
}
