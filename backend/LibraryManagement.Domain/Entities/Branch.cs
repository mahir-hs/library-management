using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Entities;

public class Branch : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public ICollection<User> Staff { get; set; } = new List<User>();
}