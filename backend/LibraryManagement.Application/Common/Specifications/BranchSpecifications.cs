using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Common.Specifications;

public class BranchByIdSpecification : SpecificationBase<Branch>
{
    public BranchByIdSpecification(Guid id)
    {
        Criteria = b => b.Id == id;
        AddInclude(b => b.BookCopies);
        AddInclude(b => b.Staff);
    }
}

public class BranchByCodeSpecification : SpecificationBase<Branch>
{
    public BranchByCodeSpecification(string code)
    {
        Criteria = b => b.Code == code;
    }
}

public class GetAllBranchesSpecification : SpecificationBase<Branch>
{
    public GetAllBranchesSpecification(int pageNumber = 1, int pageSize = 10)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(b => b.Name);
    }

    public GetAllBranchesSpecification()
    {
        ApplyOrderBy(b => b.Name);
    }
}

public class BookCopiesByBranchSpecification : SpecificationBase<BookCopy>
{
    public BookCopiesByBranchSpecification(Guid branchId)
    {
        Criteria = bc => bc.BranchId == branchId;
    }
}

public class StaffByBranchSpecification : SpecificationBase<User>
{
    public StaffByBranchSpecification(Guid branchId)
    {
        Criteria = u => u.BranchId == branchId;
    }
}

public class SearchBranchesSpecification : SpecificationBase<Branch>
{
    public SearchBranchesSpecification(
        string? name = null,
        string? code = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var criteria = PredicateBuilder.True<Branch>();

        if (!string.IsNullOrWhiteSpace(name))
        {
            criteria = criteria.And(b => b.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            criteria = criteria.And(b => b.Code.Contains(code));
        }

        if (isActive.HasValue)
        {
            criteria = criteria.And(b => b.IsActive == isActive.Value);
        }

        Criteria = criteria;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(b => b.Name);
    }

    public SearchBranchesSpecification(
        string? name = null,
        string? code = null,
        bool? isActive = null)
    {
        var criteria = PredicateBuilder.True<Branch>();

        if (!string.IsNullOrWhiteSpace(name))
        {
            criteria = criteria.And(b => b.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            criteria = criteria.And(b => b.Code.Contains(code));
        }

        if (isActive.HasValue)
        {
            criteria = criteria.And(b => b.IsActive == isActive.Value);
        }

        Criteria = criteria;
        ApplyOrderBy(b => b.Name);
    }
}
