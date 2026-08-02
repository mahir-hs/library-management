namespace LibraryManagement.Application.Common.Interfaces;


public interface IAuthService
{
    /// <summary>
    /// Validates user credentials and returns token if valid
    /// </summary>
    Task<(string AccessToken, string RefreshToken)?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<Guid> RegisterAsync(string username, string email, string password, string fullName, string phoneNumber, Guid branchId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and refreshes expired access token using refresh token
    /// </summary>
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies JWT token validity
    /// </summary>
    Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user ID from token
    /// </summary>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Gets user role from token
    /// </summary>
    string? GetUserRoleFromToken(string token);
}
