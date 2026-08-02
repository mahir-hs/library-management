using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Reservation;
using LibraryManagement.Application.Features.Reservations.Queries;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Handlers;

public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            return null;
        }

        return await ReservationMappers.MapToDtoAsync(reservation, _unitOfWork, cancellationToken);
    }
}

public class GetMyReservationsQueryHandler : IRequestHandler<GetMyReservationsQuery, PaginatedResult<MyReservationsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReservationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<MyReservationsResponse>> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetFirstAsync(
            new MemberByUserIdSpecification(_currentUserService.UserId), cancellationToken);

        if (member is null)
        {
            return new PaginatedResult<MyReservationsResponse>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var spec = new ReservationsByMemberSpecification(member.Id, request.PageNumber, request.PageSize);
        var reservations = await _unitOfWork.Reservations.GetAsync(spec, cancellationToken);
        var totalCount = await _unitOfWork.Reservations.CountAsync(
            new ReservationsByMemberSpecification(member.Id), cancellationToken);

        var dtos = new List<MyReservationsResponse>();
        foreach (var reservation in reservations)
        {
            dtos.Add(await ReservationMappers.MapToMyResponseAsync(reservation, _unitOfWork, cancellationToken));
        }

        return new PaginatedResult<MyReservationsResponse>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

public class GetReservationQueueQueryHandler : IRequestHandler<GetReservationQueueQuery, List<ReservationQueueDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationQueueQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReservationQueueDto>> Handle(GetReservationQueueQuery request, CancellationToken cancellationToken)
    {
        var spec = new ReservationsByBookSpecification(request.BookId);
        var reservations = await _unitOfWork.Reservations.GetAsync(spec, cancellationToken);

        var pendingReservations = reservations
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.ReservedAt)
            .ToList();

        var dtos = new List<ReservationQueueDto>();
        for (int i = 0; i < pendingReservations.Count; i++)
        {
            var reservation = pendingReservations[i];
            var member = await _unitOfWork.Members.GetByIdAsync(reservation.MemberId, cancellationToken);
            var user = member is not null ? await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken) : null;
            var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId, cancellationToken);

            dtos.Add(new ReservationQueueDto
            {
                Id = reservation.Id,
                MemberName = user?.FullName ?? string.Empty,
                BookTitle = book?.Title ?? string.Empty,
                ISBN = book?.ISBN ?? string.Empty,
                PositionInQueue = i + 1,
                ReservedAt = reservation.ReservedAt.DateTime,
                ExpiresAt = reservation.ExpiresAt?.DateTime,
                Status = reservation.Status
            });
        }

        return dtos;
    }
}

public static class ReservationMappers
{
    public static async Task<ReservationDto> MapToDtoAsync(Reservation reservation, IUnitOfWork uow, CancellationToken ct)
    {
        var member = await uow.Members.GetByIdAsync(reservation.MemberId, ct);
        var user = member is not null ? await uow.Users.GetByIdAsync(member.UserId, ct) : null;
        var book = await uow.Books.GetByIdAsync(reservation.BookId, ct);

        return new ReservationDto
        {
            Id = reservation.Id,
            MemberId = reservation.MemberId,
            MemberName = user?.FullName ?? string.Empty,
            BookId = reservation.BookId,
            BookTitle = book?.Title ?? string.Empty,
            ISBN = book?.ISBN ?? string.Empty,
            PositionInQueue = reservation.PositionInQueue,
            Status = reservation.Status,
            ReservedAt = reservation.ReservedAt.DateTime,
            ExpiresAt = reservation.ExpiresAt?.DateTime,
            FulfilledAt = reservation.FulfilledAt?.DateTime
        };
    }

    public static async Task<MyReservationsResponse> MapToMyResponseAsync(Reservation reservation, IUnitOfWork uow, CancellationToken ct)
    {
        var book = await uow.Books.GetByIdAsync(reservation.BookId, ct);
        var author = book is not null ? await uow.Authors.GetByIdAsync(book.AuthorId, ct) : null;
        var category = book is not null ? await uow.Categories.GetByIdAsync(book.CategoryId, ct) : null;

        return new MyReservationsResponse
        {
            Id = reservation.Id,
            BookTitle = book?.Title ?? string.Empty,
            ISBN = book?.ISBN ?? string.Empty,
            AuthorName = author?.Name ?? string.Empty,
            CategoryName = category?.Name ?? string.Empty,
            PositionInQueue = reservation.PositionInQueue,
            Status = reservation.Status,
            ReservedAt = reservation.ReservedAt.DateTime,
            ExpiresAt = reservation.ExpiresAt?.DateTime
        };
    }
}