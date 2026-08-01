namespace LibraryManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }

    /// <summary>
    /// Gets the current user's email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's role
    /// </summary>
    string? Role { get; }

    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if user has specific role
    /// </summary>
    bool HasRole(string role);
}