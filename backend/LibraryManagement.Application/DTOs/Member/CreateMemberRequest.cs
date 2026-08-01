namespace LibraryManagement.Application.DTOs.Member;

public class CreateMemberRequest
{
    public required Guid UserId { get; set; }
    public required string MembershipNumber { get; set; }
    public required string Address { get; set; }
    public string? PhoneNumber { get; set; }
}