namespace LibraryManagement.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Dictionary<string, object> GetClaims(string token);
    Guid? GetUserIdFromToken(string token);
    string? GetUserRoleFromToken(string token);
}
