namespace LibraryManagement.Application.DTOs.Borrow;

public class CreateBorrowRequest
{
    public Guid MemberId { get; set; }
    public Guid BookCopyId { get; set; }
}