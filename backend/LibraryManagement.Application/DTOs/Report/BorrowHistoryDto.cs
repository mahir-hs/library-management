namespace LibraryManagement.Application.DTOs.Report;

public class BorrowHistoryDto
{
    public Guid Id { get; set; }
    public required string MemberName { get; set; }
    public required string BookTitle { get; set; }
    public required string ISBN { get; set; }
    public required string AuthorName { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public int DaysKept { get; set; }
    public decimal FineAmount { get; set; }
}