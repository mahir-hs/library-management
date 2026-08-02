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
            // Calculate position in queue based on existing pending reservations for this book
            var queueSpec = new ReservationsByBookSpecification(request.BookId);
            var allReservations = await _unitOfWork.Reservations.GetAsync(queueSpec, cancellationToken);
            var pendingCount = allReservations.Count(r => r.Status == ReservationStatus.Pending);
            var positionInQueue = pendingCount + 1;

            var reservation = new Reservation
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                PositionInQueue = positionInQueue,
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
            PositionInQueue = existing is not null ? existing.PositionInQueue : (await _unitOfWork.Reservations.GetFirstAsync(existingSpec, cancellationToken))!.PositionInQueue,
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

public class FulfillReservationCommandHandler : IRequestHandler<FulfillReservationCommand, ReservationDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public FulfillReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto> Handle(FulfillReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            throw new NotFoundException("Reservation", request.Id);
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ConflictException("Only pending reservations can be fulfilled");
        }

        var bookCopy = await _unitOfWork.BookCopies.GetByIdAsync(request.BookCopyId, cancellationToken);
        if (bookCopy is null)
        {
            throw new NotFoundException("BookCopy", request.BookCopyId);
        }

        if (bookCopy.Status != BookCopyStatus.Available)
        {
            throw new ConflictException("Book copy is not available for reservation fulfillment");
        }

        // Fulfill the reservation
        reservation.Status = ReservationStatus.Fulfilled;
        reservation.FulfilledAt = DateTimeOffset.UtcNow;
        await _unitOfWork.Reservations.UpdateAsync(reservation, cancellationToken);

        // Set the book copy to borrowed (it's now reserved for the member)
        bookCopy.Status = BookCopyStatus.Borrowed;
        await _unitOfWork.BookCopies.UpdateAsync(bookCopy, cancellationToken);

        // Advance the queue: reassign position in queue for remaining pending reservations
        var queueSpec = new ReservationsByBookSpecification(reservation.BookId);
        var remainingReservations = await _unitOfWork.Reservations.GetAsync(queueSpec, cancellationToken);
        var pendingReservations = remainingReservations
            .Where(r => r.Status == ReservationStatus.Pending && r.Id != reservation.Id)
            .OrderBy(r => r.ReservedAt)
            .ToList();

        for (int i = 0; i < pendingReservations.Count; i++)
        {
            pendingReservations[i].PositionInQueue = i + 1;
            await _unitOfWork.Reservations.UpdateAsync(pendingReservations[i], cancellationToken);
        }

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
            PositionInQueue = reservation.PositionInQueue,
            Status = ReservationStatus.Fulfilled,
            ReservedAt = reservation.ReservedAt.DateTime,
            ExpiresAt = reservation.ExpiresAt?.DateTime,
            FulfilledAt = reservation.FulfilledAt?.DateTime
        };
    }
}