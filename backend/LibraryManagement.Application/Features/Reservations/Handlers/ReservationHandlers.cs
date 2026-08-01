using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Common.Specifications;
using LibraryManagement.Application.DTOs.Reservation;
using LibraryManagement.Application.Features.Reservations.Commands;
using LibraryManagement.Application.Features.Reservations.Queries;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Handlers;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(request.MemberId, cancellationToken);
        if (member is null)
        {
            throw new NotFoundException("Member", request.MemberId);
        }

        var book = await _unitOfWork.Books.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException("Book", request.BookId);
        }

        // Check if member already has a pending reservation for this book
        var existingSpec = new ReservationByMemberAndBookSpecification(request.MemberId, request.BookId);
        var existing = await _unitOfWork.Reservations.GetFirstAsync(existingSpec, cancellationToken);
        if (existing is not null)
        {
            if (existing.Status == ReservationStatus.Pending)
            {
                throw new ConflictException("Already have a pending reservation for this book");
            }
            // Update existing reservation
            existing.Status = ReservationStatus.Pending;
            existing.ReservedAt = DateTimeOffset.UtcNow;
            existing.ExpiresAt = DateTimeOffset.UtcNow.AddDays(3);
            await _unitOfWork.Reservations.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            var reservation = new Reservation
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                ReservedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(3),
                Status = ReservationStatus.Pending
            };
            await _unitOfWork.Reservations.AddAsync(reservation, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load user for response
        var user = await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken);

        return new ReservationDto
        {
            Id = existing is not null ? existing.Id : (await _unitOfWork.Reservations.GetFirstAsync(existingSpec, cancellationToken))!.Id,
            MemberId = request.MemberId,
            MemberName = user?.FullName ?? string.Empty,
            BookId = request.BookId,
            BookTitle = book.Title,
            ISBN = book.ISBN,
            Status = ReservationStatus.Pending,
            ReservedAt = DateTimeOffset.UtcNow.DateTime,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(3).DateTime
        };
    }
}

public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, ReservationDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            throw new NotFoundException("Reservation", request.Id);
        }

        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Fulfilled)
        {
            throw new ConflictException("Cannot cancel a reservation that is not pending or fulfilled");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _unitOfWork.Reservations.UpdateAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var member = await _unitOfWork.Members.GetByIdAsync(reservation.MemberId, cancellationToken);
        var user = member is not null ? await _unitOfWork.Users.GetByIdAsync(member.UserId, cancellationToken) : null;
        var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId, cancellationToken);

        return new ReservationDto
        {
            Id = reservation.Id,
            MemberId = reservation.MemberId,
            MemberName = user?.FullName ?? string.Empty,
            BookId = reservation.BookId,
            BookTitle = book?.Title ?? string.Empty,
            ISBN = book?.ISBN ?? string.Empty,
            Status = ReservationStatus.Cancelled,
            ReservedAt = reservation.ReservedAt.DateTime,
            ExpiresAt = reservation.ExpiresAt?.DateTime
        };
    }
}