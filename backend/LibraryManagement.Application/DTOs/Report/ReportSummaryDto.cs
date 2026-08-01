namespace LibraryManagement.Application.DTOs.Report;

public class ReportSummaryDto
{
    public int TotalBooks { get; set; }
    public int TotalAvailableCopies { get; set; }
    public int TotalBorrowedCopies { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int PendingReservations { get; set; }
    public decimal TotalOutstandingFines { get; set; }
}