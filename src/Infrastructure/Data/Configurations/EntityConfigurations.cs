using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data.Configurations;

/// <summary>EF Core entity type configuration for Product.</summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseIdentityColumn();

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.CreatedOn)
            .IsRequired();

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(p => p.ModifiedOn)
            .IsRequired(false);

        // Index on ProductName for faster lookups
        builder.HasIndex(p => p.ProductName)
            .HasDatabaseName("IX_Product_ProductName");

        // One-to-many: Product -> Items
        builder.HasMany(p => p.Items)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>EF Core entity type configuration for Item.</summary>
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Item");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .UseIdentityColumn();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.ProductId)
            .IsRequired();

        // Index on ProductId for efficient joins
        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("IX_Item_ProductId");
    }
}
