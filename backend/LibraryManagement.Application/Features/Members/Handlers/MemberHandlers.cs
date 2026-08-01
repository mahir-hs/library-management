using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Member;
using LibraryManagement.Application.Features.Members.Commands;
using LibraryManagement.Application.Features.Members.Queries;
using LibraryManagement.Domain.Entities;
using MediatR;

namespace LibraryManagement.Application.Features.Members.Handlers;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDetailDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        // Check if member already exists for this user
        var existingSpec = new MemberByUserIdSpecification(request.UserId);
        var existing = await _unitOfWork.Members.GetFirstAsync(existingSpec, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("Member already exists for this user");
        }

        // Check if membership number already exists
        var existingByNumberSpec = new MemberByMembershipNumberSpecification(request.MembershipNumber);
        var existingByNumber = await _unitOfWork.Members.GetFirstAsync(existingByNumberSpec, cancellationToken);
        if (existingByNumber is not null)
        {
            throw new ConflictException($"Membership number '{request.MembershipNumber}' already exists");
        }

        var member = new Member
        {
            UserId = request.UserId,
            MembershipNumber = request.MembershipNumber,
            Address = request.Address,
            JoinedDate = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Members.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MemberMappers.MapToDetailDtoAsync(member, _unitOfWork, cancellationToken);
    }
}

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.Id, cancellationToken);
        if (member is null)
        {
            throw new NotFoundException("Member", request.Id);
        }

        if (request.MembershipNumber is not null)
        {
            var existingSpec = new MemberByMembershipNumberSpecification(request.MembershipNumber);
            var existing = await _unitOfWork.Members.GetFirstAsync(existingSpec, cancellationToken);
            if (existing is not null && existing.Id != request.Id)
            {
                throw new ConflictException($"Membership number '{request.MembershipNumber}' already in use");
            }
            member.MembershipNumber = request.MembershipNumber;
        }

        if (request.Address is not null)
        {
            member.Address = request.Address;
        }

        if (request.PhoneNumber is not null)
        {
            member.User.PhoneNumber = request.PhoneNumber;
            await _unitOfWork.Users.UpdateAsync(member.User, cancellationToken);
        }

        await _unitOfWork.Members.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MemberMappers.MapToDtoAsync(member, _unitOfWork, cancellationToken);
    }
}

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.Id, cancellationToken);
        if (member is null)
        {
            throw new NotFoundException("Member", request.Id);
        }

        await _unitOfWork.Members.DeleteAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}