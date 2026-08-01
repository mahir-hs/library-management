using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Branch;
using LibraryManagement.Application.Features.Branchs.Queries;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Branchs.Handlers;

public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBranchByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.Branchs.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
        {
            return null;
        }

        var copiesSpec = new BookCopiesByBranchSpecification(request.Id);
        var copies = await _unitOfWork.BookCopies.GetAsync(copiesSpec, cancellationToken);

        var staffSpec = new StaffByBranchSpecification(request.Id);
        var staff = await _unitOfWork.Users.GetAsync(staffSpec, cancellationToken);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Code = branch.Code,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive,
            BookCopyCount = copies.Count,
            StaffCount = staff.Count,
            CreatedAt = branch.CreatedAt.DateTime
        };
    }
}

public class GetBranchByCodeQueryHandler : IRequestHandler<GetBranchByCodeQuery, BranchDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBranchByCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchDto?> Handle(GetBranchByCodeQuery request, CancellationToken cancellationToken)
    {
        var spec = new BranchByCodeSpecification(request.Code);
        var branch = await _unitOfWork.Branchs.GetFirstAsync(spec, cancellationToken);
        if (branch is null)
        {
            return null;
        }

        var copiesSpec = new BookCopiesByBranchSpecification(branch.Id);
        var copies = await _unitOfWork.BookCopies.GetAsync(copiesSpec, cancellationToken);

        var staffSpec = new StaffByBranchSpecification(branch.Id);
        var staff = await _unitOfWork.Users.GetAsync(staffSpec, cancellationToken);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Code = branch.Code,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive,
            BookCopyCount = copies.Count,
            StaffCount = staff.Count,
            CreatedAt = branch.CreatedAt.DateTime
        };
    }
}

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, PaginatedResult<BranchDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBranchesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllBranchesSpecification(request.PageNumber, request.PageSize);
        var branches = await _unitOfWork.Branchs.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Branchs.CountAsync(new GetAllBranchesSpecification(), cancellationToken);

        var branchDtos = new List<BranchDto>();
        foreach (var branch in branches)
        {
            var copiesSpec = new BookCopiesByBranchSpecification(branch.Id);
            var copies = await _unitOfWork.BookCopies.GetAsync(copiesSpec, cancellationToken);

            var staffSpec = new StaffByBranchSpecification(branch.Id);
            var staff = await _unitOfWork.Users.GetAsync(staffSpec, cancellationToken);

            branchDtos.Add(new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Code = branch.Code,
                Address = branch.Address,
                Phone = branch.Phone,
                Email = branch.Email,
                IsActive = branch.IsActive,
                BookCopyCount = copies.Count,
                StaffCount = staff.Count,
                CreatedAt = branch.CreatedAt.DateTime
            });
        }

        return new PaginatedResult<BranchDto>
        {
            Items = branchDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class SearchBranchesQueryHandler : IRequestHandler<SearchBranchesQuery, PaginatedResult<BranchSearchResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchBranchesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<BranchSearchResponse>> Handle(SearchBranchesQuery request, CancellationToken cancellationToken)
    {
        var spec = new SearchBranchesSpecification(
            request.Name, request.Code, request.IsActive,
            request.PageNumber, request.PageSize);

        var branches = await _unitOfWork.Branchs.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Branchs.CountAsync(
            new SearchBranchesSpecification(request.Name, request.Code, request.IsActive),
            cancellationToken);

        var results = new List<BranchSearchResponse>();
        foreach (var branch in branches)
        {
            var copiesSpec = new BookCopiesByBranchSpecification(branch.Id);
            var copies = await _unitOfWork.BookCopies.GetAsync(copiesSpec, cancellationToken);

            results.Add(new BranchSearchResponse
            {
                Id = branch.Id,
                Name = branch.Name,
                Code = branch.Code,
                Address = branch.Address,
                Phone = branch.Phone,
                Email = branch.Email,
                IsActive = branch.IsActive,
                BookCopyCount = copies.Count
            });
        }

        return new PaginatedResult<BranchSearchResponse>
        {
            Items = results,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
