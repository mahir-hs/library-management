using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Common.Specifications;

public class BorrowRecordsByStatusSpecification : SpecificationBase<BorrowRecord>
{
    public BorrowRecordsByStatusSpecification(BorrowStatus status, int pageNumber = 1, int pageSize = int.MaxValue)
    {
        Criteria = br => br.Status == status;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public BorrowRecordsByStatusSpecification(BorrowStatus status)
    {
        Criteria = br => br.Status == status;
    }
}

public class ReservationsByStatusSpecification : SpecificationBase<Reservation>
{
    public ReservationsByStatusSpecification(ReservationStatus status, int pageNumber = 1, int pageSize = int.MaxValue)
    {
        Criteria = r => r.Status == status;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public ReservationsByStatusSpecification(ReservationStatus status)
    {
        Criteria = r => r.Status == status;
    }
}

public class GetAllSpecification<T> : SpecificationBase<T>
{
    public GetAllSpecification(int pageNumber = 1, int pageSize = int.MaxValue)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public GetAllSpecification()
    {
    }
}

public class BookCopiesByStatusSpecification : SpecificationBase<BookCopy>
{
    public BookCopiesByStatusSpecification(BookCopyStatus status, int pageNumber = 1, int pageSize = int.MaxValue)
    {
        Criteria = bc => bc.Status == status;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public BookCopiesByStatusSpecification(BookCopyStatus status)
    {
        Criteria = bc => bc.Status == status;
    }
}

public class BorrowHistorySpecification : SpecificationBase<BorrowRecord>
{
    public BorrowHistorySpecification(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var criteria = PredicateBuilder.True<BorrowRecord>();

        if (fromDate.HasValue)
        {
            criteria = criteria.And(br => br.BorrowedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            criteria = criteria.And(br => br.BorrowedAt <= toDate.Value);
        }

        Criteria = criteria;
        AddInclude(br => br.Member);
        AddInclude("Member.User");
        AddInclude(br => br.BookCopy);
        AddInclude("BookCopy.Book");
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(br => br.BorrowedAt);
    }

    public BorrowHistorySpecification(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var criteria = PredicateBuilder.True<BorrowRecord>();

        if (fromDate.HasValue)
        {
            criteria = criteria.And(br => br.BorrowedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            criteria = criteria.And(br => br.BorrowedAt <= toDate.Value);
        }

        Criteria = criteria;
        AddInclude(br => br.Member);
        AddInclude("Member.User");
        AddInclude(br => br.BookCopy);
        AddInclude("BookCopy.Book");
        ApplyOrderByDescending(br => br.BorrowedAt);
    }
}