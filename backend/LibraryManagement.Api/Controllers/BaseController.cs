namespace LibraryManagement.API.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Base controller with common functionality
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets current user ID from claims.
    /// Checks ClaimTypes.NameIdentifier first (JWT Bearer default mapping for "sub"),
    /// then falls back to the raw "sub" claim type.
    /// </summary>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Gets current user role from claims.
    /// Checks ClaimTypes.Role first (JWT Bearer default mapping for "role"),
    /// then falls back to the raw "role" claim type.
    /// </summary>
    protected string? GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirst("role")?.Value;
    }

    /// <summary>
    /// Gets current user email from claims.
    /// Checks ClaimTypes.Email first (JWT Bearer default mapping for "email"),
    /// then falls back to the raw "email" claim type.
    /// </summary>
    protected string? GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Checks if current user has specified role
    /// </summary>
    protected bool HasRole(params string[] roles)
    {
        var userRole = GetCurrentUserRole();
        return userRole != null && roles.Contains(userRole, StringComparer.OrdinalIgnoreCase);
    }
}