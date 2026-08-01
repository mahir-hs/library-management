using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Common.Specifications;

public class MemberByUserIdSpecification : SpecificationBase<Member>
{
    public MemberByUserIdSpecification(Guid userId)
    {
        Criteria = m => m.UserId == userId;
        AddInclude(m => m.User);
    }
}

public class MemberByMembershipNumberSpecification : SpecificationBase<Member>
{
    public MemberByMembershipNumberSpecification(string membershipNumber)
    {
        Criteria = m => m.MembershipNumber == membershipNumber;
        AddInclude(m => m.User);
    }
}

public class GetAllMembersSpecification : SpecificationBase<Member>
{
    public GetAllMembersSpecification(int pageNumber = 1, int pageSize = 10)
    {
        AddInclude(m => m.User);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(m => m.User.FullName);
    }

    public GetAllMembersSpecification()
    {
        AddInclude(m => m.User);
        ApplyOrderBy(m => m.User.FullName);
    }
}