using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Application.Mapping;
using ProductsAPI.Application.Services;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Domain.Exceptions;
using Xunit;

namespace ProductsAPI.Application.Tests;

/// <summary>Unit tests for ProductService using Moq and xUnit.</summary>
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly Mock<ILogger<ProductService>> _loggerMock = new();
    private readonly IMapper _mapper;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _uowMock.Setup(u => u.Products).Returns(_repoMock.Object);
        _sut = new ProductService(_uowMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingProduct_ReturnsProductDto()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Widget", CreatedBy = "test@test.com", CreatedOn = DateTime.UtcNow };
        _repoMock.Setup(r => r.GetByIdWithItemsAsync(1, default)).ReturnsAsync(product);

        // Act
        var result = await _sut.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Widget", result.ProductName);
    }

    [Fact]
    public async Task GetProductByIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdWithItemsAsync(99, default)).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetProductByIdAsync(99));
    }

    [Fact]
    public async Task CreateProductAsync_ValidDto_ReturnsCreatedProduct()
    {
        // Arrange
        var dto = new CreateProductDto { ProductName = "New Product" };
        var product = new Product { Id = 5, ProductName = "New Product", CreatedBy = "user@test.com", CreatedOn = DateTime.UtcNow };
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), default)).ReturnsAsync(product);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.CreateProductAsync(dto, "user@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.ProductName);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateProductAsync(99, new UpdateProductDto { ProductName = "X" }, "user"));
    }

    [Fact]
    public async Task UpdateProductAsync_ExistingProduct_UpdatesAndReturns()
    {
        // Arrange
        var existing = new Product { Id = 1, ProductName = "Old", CreatedBy = "user", CreatedOn = DateTime.UtcNow };
        _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existing);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateProductAsync(1, new UpdateProductDto { ProductName = "New" }, "editor");

        // Assert
        Assert.Equal("New", result.ProductName);
        Assert.Equal("editor", result.ModifiedBy);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_Deletes()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Widget", CreatedBy = "x", CreatedOn = DateTime.UtcNow };
        _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _sut.DeleteProductAsync(1);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(product, default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, ProductName = "P1", CreatedBy = "x", CreatedOn = DateTime.UtcNow },
            new() { Id = 2, ProductName = "P2", CreatedBy = "x", CreatedOn = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAllAsync(1, 10, default)).ReturnsAsync(
            new PagedResult<Product> { Items = products, TotalCount = 2, Page = 1, PageSize = 10 });

        // Act
        var result = await _sut.GetAllProductsAsync(1, 10);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}
