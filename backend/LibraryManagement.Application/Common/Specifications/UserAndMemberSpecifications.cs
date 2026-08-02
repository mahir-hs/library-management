using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Common.Specifications;

public class UserByIdSpecification : SpecificationBase<User>
{
    public UserByIdSpecification(Guid id)
    {
        Criteria = u => u.Id == id;
    }
}

public class GetAllUsersSpecification : SpecificationBase<User>
{
    public GetAllUsersSpecification(int pageNumber = 1, int pageSize = 10)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(u => u.Username);
    }

    public GetAllUsersSpecification()
    {
        ApplyOrderBy(u => u.Username);
    }
}