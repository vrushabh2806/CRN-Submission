namespace ProductsAPI.Application.DTOs;

/// <summary>Response DTO for a Product.</summary>
public class ProductDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}

/// <summary>Request DTO for creating a product.</summary>
public class CreateProductDto
{
    public string ProductName { get; set; } = string.Empty;
}

/// <summary>Request DTO for updating a product.</summary>
public class UpdateProductDto
{
    public string ProductName { get; set; } = string.Empty;
}

/// <summary>Response DTO for an Item.</summary>
public class ItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>Request DTO for creating an item.</summary>
public class CreateItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>Request DTO for updating an item.</summary>
public class UpdateItemDto
{
    public int Quantity { get; set; }
}
