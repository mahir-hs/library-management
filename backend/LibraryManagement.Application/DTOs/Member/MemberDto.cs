namespace LibraryManagement.Application.DTOs.Member;

public class MemberDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string MembershipNumber { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Address { get; set; }
    public DateTime JoinedDate { get; set; }
    public int ActiveBorrows { get; set; }
    public int TotalBorrows { get; set; }
    public int OverdueBorrows { get; set; }
}