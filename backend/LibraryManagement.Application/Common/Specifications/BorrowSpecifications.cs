using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Common.Specifications;

public class BorrowRecordsByMemberSpecification : SpecificationBase<BorrowRecord>
{
    public BorrowRecordsByMemberSpecification(Guid memberId, int pageNumber = 1, int pageSize = 10)
    {
        Criteria = br => br.MemberId == memberId;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(br => br.BorrowedAt);
    }

    public BorrowRecordsByMemberSpecification(Guid memberId)
    {
        Criteria = br => br.MemberId == memberId;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        ApplyOrderByDescending(br => br.BorrowedAt);
    }
}

public class BorrowRecordsByBookCopySpecification : SpecificationBase<BorrowRecord>
{
    public BorrowRecordsByBookCopySpecification(Guid bookCopyId, int pageNumber = 1, int pageSize = 10)
    {
        Criteria = br => br.BookCopyId == bookCopyId;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        AddInclude("BookCopy.Book");
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(br => br.BorrowedAt);
    }

    public BorrowRecordsByBookCopySpecification(Guid bookCopyId)
    {
        Criteria = br => br.BookCopyId == bookCopyId;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        AddInclude("BookCopy.Book");
        ApplyOrderByDescending(br => br.BorrowedAt);
    }
}

public class OverdueBorrowsSpecification : SpecificationBase<BorrowRecord>
{
    public OverdueBorrowsSpecification(int pageNumber = 1, int pageSize = 10)
    {
        Criteria = br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(br => br.DueDate);
    }

    public OverdueBorrowsSpecification()
    {
        Criteria = br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
        ApplyOrderBy(br => br.DueDate);
    }
}

public class OverdueBorrowsByMemberSpecification : SpecificationBase<BorrowRecord>
{
    public OverdueBorrowsByMemberSpecification(Guid memberId)
    {
        Criteria = br => br.MemberId == memberId && br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow;
        AddInclude(br => br.Member);
        AddInclude(br => br.BookCopy);
    }
}

public class ActiveBorrowsByMemberSpecification : SpecificationBase<BorrowRecord>
{
    public ActiveBorrowsByMemberSpecification(Guid memberId)
    {
        Criteria = br => br.MemberId == memberId && br.Status == BorrowStatus.Borrowed;
    }
}