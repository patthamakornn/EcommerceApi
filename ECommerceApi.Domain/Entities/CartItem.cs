namespace ECommerceApi.Domain.Entities;

public partial class CartItem
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
