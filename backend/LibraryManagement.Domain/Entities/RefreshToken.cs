namespace LibraryManagement.Domain.Entities;

using Common;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public User User { get; set; } = null!;
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
}