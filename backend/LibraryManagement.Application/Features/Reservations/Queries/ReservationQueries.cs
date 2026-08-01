using LibraryManagement.Application.DTOs.Reservation;
using LibraryManagement.Domain.Common;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Queries;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto?>;

public record GetMyReservationsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<MyReservationsResponse>>;

public record GetReservationQueueQuery(Guid BookId) : IRequest<List<ReservationQueueDto>>;