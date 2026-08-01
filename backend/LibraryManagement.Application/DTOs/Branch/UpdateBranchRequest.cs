namespace LibraryManagement.Application.DTOs.Branch;

public class UpdateBranchRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}
