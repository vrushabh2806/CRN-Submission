using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using System.Security.Claims;

namespace ProductsAPI.API.Controllers;

/// <summary>
/// RESTful controller for Product CRUD operations.
/// All endpoints require JWT authentication; Delete is restricted to Admin role.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>Gets a paginated list of all products.</summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 10, max 100).</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        pageSize = Math.Min(pageSize, 100);
        var result = await _productService.GetAllProductsAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result, $"Found {result.TotalCount} products."));
    }

    /// <summary>Gets a specific product by ID, including its items.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var product = await _productService.GetProductByIdAsync(id, ct);
        return Ok(ApiResponse<ProductDto>.Ok(product));
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct = default)
    {
        var createdBy = User.FindFirstValue(ClaimTypes.Email) ?? "system";
        var product = await _productService.CreateProductAsync(dto, createdBy, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            ApiResponse<ProductDto>.Ok(product, "Product created successfully."));
    }

    /// <summary>Updates an existing product.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,User")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct = default)
    {
        var modifiedBy = User.FindFirstValue(ClaimTypes.Email) ?? "system";
        var product = await _productService.UpdateProductAsync(id, dto, modifiedBy, ct);
        return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated successfully."));
    }

    /// <summary>Deletes a product by ID (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        await _productService.DeleteProductAsync(id, ct);
        return NoContent();
    }
}
