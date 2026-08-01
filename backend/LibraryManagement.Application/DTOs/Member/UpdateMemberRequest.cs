namespace LibraryManagement.Application.DTOs.Member;

public class UpdateMemberRequest
{
    public string? MembershipNumber { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}