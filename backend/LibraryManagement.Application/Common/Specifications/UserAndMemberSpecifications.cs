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