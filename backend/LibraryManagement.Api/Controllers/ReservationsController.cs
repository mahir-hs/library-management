namespace LibraryManagement.API.Controllers;

using Application.DTOs.Reservation;
using Application.Features.Reservations.Commands;
using Application.Features.Reservations.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Reservation management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class ReservationsController : BaseController
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Reserve a book
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateReservationCommand(request.MemberId, request.BookId);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetReservationById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelReservation(
        Guid id,
        [FromBody] CancelReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CancelReservationCommand(id, request.Reason);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Fulfill a reservation (assign a book copy to the reserving member)
    /// </summary>
    [HttpPut("{id:guid}/fulfill")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FulfillReservation(
        Guid id,
        [FromBody] FulfillReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new FulfillReservationCommand(id, request.BookCopyId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReservationById(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReservationByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all reservations for the current member
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PaginatedResult<MyReservationsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReservations(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyReservationsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get reservation queue for a book
    /// </summary>
    [HttpGet("queue/{bookId:guid}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(List<ReservationQueueDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationQueue(
        Guid bookId, CancellationToken cancellationToken)
    {
        var query = new GetReservationQueueQuery(bookId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}