namespace LibraryManagement.Application.DTOs.Branch;

public class CreateBranchRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
