namespace ECommerceApi.Domain.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
