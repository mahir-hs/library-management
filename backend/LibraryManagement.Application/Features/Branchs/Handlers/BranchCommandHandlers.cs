using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Branch;
using LibraryManagement.Application.Features.Branchs.Commands;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Branchs.Handlers;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        // Check if branch code already exists
        var codeSpec = new BranchByCodeSpecification(request.Code);
        var existing = await _unitOfWork.Branchs.GetFirstAsync(codeSpec, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"Branch with code '{request.Code}' already exists");
        }

        var branch = new Branch
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Branchs.AddAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Code = branch.Code,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive,
            BookCopyCount = 0,
            StaffCount = 0,
            CreatedAt = branch.CreatedAt.DateTime
        };
    }
}

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, BranchDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBranchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchDto> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.Branchs.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch", request.Id);
        }

        // Check if branch code already exists (for other branches)
        var codeSpec = new BranchByCodeSpecification(request.Code);
        var existing = await _unitOfWork.Branchs.GetFirstAsync(codeSpec, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new ConflictException($"Branch code '{request.Code}' is already in use by another branch");
        }

        branch.Name = request.Name;
        branch.Code = request.Code;
        branch.Address = request.Address;
        branch.Phone = request.Phone;
        branch.Email = request.Email;
        branch.IsActive = request.IsActive;
        branch.UpdatedAt = DateTimeOffset.UtcNow;

        await _unitOfWork.Branchs.UpdateAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Code = branch.Code,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            IsActive = branch.IsActive,
            BookCopyCount = branch.BookCopies.Count,
            StaffCount = branch.Staff.Count,
            CreatedAt = branch.CreatedAt.DateTime
        };
    }
}

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBranchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.Branchs.GetByIdAsync(request.Id, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch", request.Id);
        }

        // Check if branch has book copies assigned
        var bookCopiesSpec = new BookCopiesByBranchSpecification(request.Id);
        var bookCopies = await _unitOfWork.BookCopies.GetAsync(bookCopiesSpec, cancellationToken);
        if (bookCopies.Count > 0)
        {
            throw new ConflictException("Cannot delete a branch that has book copies assigned. Reassign them first.");
        }

        // Check if branch has staff members
        var staffSpec = new StaffByBranchSpecification(request.Id);
        var staff = await _unitOfWork.Users.GetAsync(staffSpec, cancellationToken);
        if (staff.Count > 0)
        {
            throw new ConflictException("Cannot delete a branch that has staff members. Reassign them first.");
        }

        await _unitOfWork.Branchs.DeleteAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
