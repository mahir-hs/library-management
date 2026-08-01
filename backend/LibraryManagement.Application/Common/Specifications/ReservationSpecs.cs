using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Common.Specifications;

public class ReservationByMemberAndBookSpecification : SpecificationBase<Reservation>
{
    public ReservationByMemberAndBookSpecification(Guid memberId, Guid bookId)
    {
        Criteria = r => r.MemberId == memberId && r.BookId == bookId;
    }
}

// ReservationsByMemberSpecification is already defined in ReservationSpecifications.cs

public class ReservationsByBookSpecification : SpecificationBase<Reservation>
{
    public ReservationsByBookSpecification(Guid bookId)
    {
        Criteria = r => r.BookId == bookId;
        ApplyOrderBy(r => r.ReservedAt);
    }
}