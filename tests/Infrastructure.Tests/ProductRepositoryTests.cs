using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Data;
using ProductsAPI.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductsAPI.Infrastructure.Tests;

/// <summary>Integration tests for ProductRepository using EF Core InMemory provider.</summary>
public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repo;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _repo = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_AddsProductToDatabase()
    {
        // Arrange
        var product = new Product { ProductName = "Widget", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

        // Act
        await _repo.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var product = new Product { ProductName = "Gadget", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Gadget", result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
    {
        var result = await _repo.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedResults()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
            _context.Products.Add(new Product { ProductName = $"P{i}", CreatedBy = "test", CreatedOn = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repo.GetAllAsync(2, 5);

        // Assert
        Assert.Equal(15, result.TotalCount);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(2, result.Page);
    }

    [Fact]
    public async Task ExistsAsync_ExistingProduct_ReturnsTrue()
    {
        var product = new Product { ProductName = "Test", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var exists = await _repo.ExistsAsync(product.Id);
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingProduct_ReturnsFalse()
    {
        var exists = await _repo.ExistsAsync(999);
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        var product = new Product { ProductName = "ToDelete", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _repo.DeleteAsync(product);
        await _context.SaveChangesAsync();

        Assert.Equal(0, await _context.Products.CountAsync());
    }

    public void Dispose() => _context.Dispose();
}
