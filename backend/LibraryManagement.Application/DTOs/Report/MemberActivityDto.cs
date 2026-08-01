namespace LibraryManagement.Application.DTOs.Report;

public class MemberActivityDto
{
    public Guid MemberId { get; set; }
    public required string MemberName { get; set; }
    public required string MembershipNumber { get; set; }
    public int TotalBorrows { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int PendingReservations { get; set; }
    public decimal TotalFines { get; set; }
    public DateTime JoinedDate { get; set; }
}
