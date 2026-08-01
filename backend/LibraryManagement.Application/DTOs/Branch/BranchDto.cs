namespace LibraryManagement.Application.DTOs.Branch;

public class BranchDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public int BookCopyCount { get; set; }
    public int StaffCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
