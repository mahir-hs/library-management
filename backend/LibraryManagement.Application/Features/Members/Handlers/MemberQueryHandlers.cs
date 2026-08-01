using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Member;
using LibraryManagement.Application.Features.Members.Queries;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Members.Handlers;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMemberByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDetailDto?> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.Id, cancellationToken);
        if (member is null)
        {
            return null;
        }

        return await MemberMappers.MapToDetailDtoAsync(member, _unitOfWork, cancellationToken);
    }
}

public class GetMemberByUserIdQueryHandler : IRequestHandler<GetMemberByUserIdQuery, MemberDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMemberByUserIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDetailDto?> Handle(GetMemberByUserIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new MemberByUserIdSpecification(request.UserId);
        var member = await _unitOfWork.Members.GetFirstAsync(spec, cancellationToken);
        if (member is null)
        {
            return null;
        }

        return await MemberMappers.MapToDetailDtoAsync(member, _unitOfWork, cancellationToken);
    }
}

public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, PaginatedResult<MemberDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMembersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<MemberDto>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllMembersSpecification(request.PageNumber, request.PageSize);
        var members = await _unitOfWork.Members.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Members.CountAsync(
            new GetAllMembersSpecification(), cancellationToken);

        var dtos = new List<MemberDto>();
        foreach (var member in members)
        {
            dtos.Add(await MemberMappers.MapToDtoAsync(member, _unitOfWork, cancellationToken));
        }

        return new PaginatedResult<MemberDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public static class MemberMappers
{
    public static async Task<MemberDetailDto> MapToDetailDtoAsync(Member member, IUnitOfWork uow, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(member.UserId, ct);

        // Count borrows
        var borrowSpec = new BorrowRecordsByMemberSpecification(member.Id);
        var borrowRecords = await uow.BorrowRecords.GetAsync(borrowSpec, ct);
        int activeBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed);
        int totalBorrows = borrowRecords.Count;
        int overdueBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow);

        // Count pending reservations
        var reservationSpec = new ReservationsByMemberSpecification(member.Id);
        var reservations = await uow.Reservations.GetAsync(reservationSpec, ct);
        int pendingReservations = reservations.Count(r => r.Status == ReservationStatus.Pending);

        return new MemberDetailDto
        {
            Id = member.Id,
            MembershipNumber = member.MembershipNumber,
            FullName = user?.FullName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            PhoneNumber = user?.PhoneNumber,
            Address = member.Address,
            JoinedDate = member.JoinedDate.DateTime,
            ActiveBorrows = activeBorrows,
            TotalBorrows = totalBorrows,
            OverdueBorrows = overdueBorrows,
            PendingReservations = pendingReservations
        };
    }

    public static async Task<MemberDto> MapToDtoAsync(Member member, IUnitOfWork uow, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(member.UserId, ct);

        var borrowSpec = new BorrowRecordsByMemberSpecification(member.Id);
        var borrowRecords = await uow.BorrowRecords.GetAsync(borrowSpec, ct);
        int activeBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed);
        int totalBorrows = borrowRecords.Count;
        int overdueBorrows = borrowRecords.Count(br => br.Status == BorrowStatus.Borrowed && br.DueDate < DateTimeOffset.UtcNow);

        return new MemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            MembershipNumber = member.MembershipNumber,
            FullName = user?.FullName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            PhoneNumber = user?.PhoneNumber,
            Address = member.Address,
            JoinedDate = member.JoinedDate.DateTime,
            ActiveBorrows = activeBorrows,
            TotalBorrows = totalBorrows,
            OverdueBorrows = overdueBorrows
        };
    }
}