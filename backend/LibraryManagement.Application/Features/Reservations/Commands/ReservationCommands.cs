using LibraryManagement.Application.DTOs.Reservation;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Commands;

public record CreateReservationCommand(Guid MemberId, Guid BookId) : IRequest<ReservationDto>;

public record CancelReservationCommand(Guid Id, string Reason) : IRequest<ReservationDto>;