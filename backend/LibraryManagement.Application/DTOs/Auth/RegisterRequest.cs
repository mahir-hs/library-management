namespace LibraryManagement.Application.DTOs.Auth;

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public required Guid BranchId { get; set; }
    public required string Role { get; set; }
}
