namespace LibraryManagement.Domain.Entities;

using Common;
using Enums;

public sealed class User : AuditableEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }
    public Member? Member { get; set; }
    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens;
}