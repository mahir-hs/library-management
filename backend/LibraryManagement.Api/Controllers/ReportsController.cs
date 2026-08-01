namespace LibraryManagement.API.Controllers;

using Application.DTOs.Report;
using Application.Features.Reports.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Report endpoints for admin and librarian views
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : BaseController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get dashboard summary with key metrics
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ReportSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportSummary(CancellationToken cancellationToken)
    {
        var query = new GetReportSummaryQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get borrow history with optional date range filter
    /// </summary>
    [HttpGet("borrow-history")]
    [ProducesResponseType(typeof(PaginatedResult<BorrowHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBorrowHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBorrowHistoryQuery(pageNumber, pageSize, fromDate, toDate);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get overdue books report
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(PaginatedResult<OverdueBookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOverdueBooksQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get activity for a specific member
    /// </summary>
    [HttpGet("member-activity/{memberId:guid}")]
    [ProducesResponseType(typeof(MemberActivityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberActivity(
        Guid memberId, CancellationToken cancellationToken)
    {
        var query = new GetMemberActivityQuery(memberId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}