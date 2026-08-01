namespace LibraryManagement.Infrastructure.Services;

using System.Security.Claims;
using LibraryManagement.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            // JWT Bearer maps "sub" to ClaimTypes.NameIdentifier by default;
            // check the mapped type first, then fall back to the raw claim type.
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier)
              ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    public string? Email
    {
        get
        {
            // JWT Bearer maps "email" to ClaimTypes.Email by default
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Email)
              ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("email");
        }
    }

    public string? Role
    {
        get
        {
            // JWT Bearer maps "role" to ClaimTypes.Role by default
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Role)
              ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("role");
        }
    }

    public bool IsAuthenticated
    {
        get => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }

    public bool HasRole(string role)
    {
        var userRole = Role;
        return userRole != null && userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
    }
}