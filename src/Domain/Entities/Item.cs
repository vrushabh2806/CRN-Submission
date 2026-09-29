namespace ProductsAPI.Domain.Entities;

/// <summary>
/// Represents an item associated with a product.
/// </summary>
public class Item
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    // Navigation property
    public Product Product { get; set; } = null!;
}
