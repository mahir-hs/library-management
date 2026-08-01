namespace LibraryManagement.Application.DTOs.Report;

public class OverdueBookDto
{
    public Guid BorrowId { get; set; }
    public required string MemberName { get; set; }
    public required string BookTitle { get; set; }
    public required string ISBN { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public decimal EstimatedFine { get; set; }
}