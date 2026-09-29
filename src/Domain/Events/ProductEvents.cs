namespace ProductsAPI.Domain.Events;

/// <summary>
/// Base class for all domain events.
/// </summary>
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Raised when a new product is created.
/// </summary>
public class ProductCreatedEvent : DomainEvent
{
    public int ProductId { get; }
    public string ProductName { get; }

    public ProductCreatedEvent(int productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

/// <summary>
/// Raised when a product is deleted.
/// </summary>
public class ProductDeletedEvent : DomainEvent
{
    public int ProductId { get; }

    public ProductDeletedEvent(int productId)
    {
        ProductId = productId;
    }
}
