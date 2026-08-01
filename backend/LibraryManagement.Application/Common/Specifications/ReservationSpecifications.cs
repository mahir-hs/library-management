using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Common.Specifications;

public class ReservationsByMemberSpecification : SpecificationBase<Reservation>
{
    public ReservationsByMemberSpecification(Guid memberId, int pageNumber = 1, int pageSize = 10)
    {
        Criteria = r => r.MemberId == memberId;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(r => r.ReservedAt);
    }

    public ReservationsByMemberSpecification(Guid memberId)
    {
        Criteria = r => r.MemberId == memberId;
        ApplyOrderByDescending(r => r.ReservedAt);
    }
}