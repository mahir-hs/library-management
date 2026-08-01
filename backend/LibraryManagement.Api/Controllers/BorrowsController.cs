namespace LibraryManagement.API.Controllers;

using Application.DTOs.Borrow;
using Application.Features.Borrows.Commands;
using Application.Features.Borrows.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Borrow management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class BorrowsController : BaseController
{
    private readonly IMediator _mediator;

    public BorrowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Borrow a book copy
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BorrowDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BorrowBook(
        [FromBody] CreateBorrowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new BorrowBookCommand(request.MemberId, request.BookCopyId);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBorrowById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Return a borrowed book copy
    /// </summary>
    [HttpPut("{id:guid}/return")]
    [ProducesResponseType(typeof(BorrowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReturnBook(
        Guid id,
        [FromBody] ReturnBorrowRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ReturnBookCommand(id, request.FineAmount);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get borrow record by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BorrowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBorrowById(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBorrowByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get borrows for a specific member
    /// </summary>
    [HttpGet("member/{memberId:guid}")]
    [ProducesResponseType(typeof(PaginatedResult<BorrowDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBorrowsByMember(
        Guid memberId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBorrowsByMemberQuery(memberId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all borrows for the current member
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PaginatedResult<MyBorrowsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBorrows(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyBorrowsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all overdue borrows (Admin/Librarian only)
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(PaginatedResult<BorrowListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueBorrows(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOverdueBorrowsQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}