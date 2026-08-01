namespace LibraryManagement.API.Controllers;

using Application.DTOs.Branch;
using Application.Features.Branchs.Commands;
using Application.Features.Branchs.Queries;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Branch management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class BranchesController : BaseController
{
    private readonly IMediator _mediator;

    public BranchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new branch
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBranch(
        [FromBody] CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBranchCommand(
            request.Name, request.Code, request.Address,
            request.Phone, request.Email
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBranchById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a branch by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchById(
        Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBranchByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a branch by code
    /// </summary>
    [HttpGet("code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranchByCode(
        string code, CancellationToken cancellationToken)
    {
        var query = new GetBranchByCodeQuery(code);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all branches with pagination
    /// </summary>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<BranchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBranches(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBranchesQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search branches by name, code, or active status
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<BranchSearchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBranches(
        [FromQuery] string? name,
        [FromQuery] string? code,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchBranchesQuery(name, code, isActive, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update a branch
    /// </summary>
    /// <remarks>Requires Admin or Librarian role</remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(BranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBranch(
        Guid id,
        [FromBody] UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBranchCommand(
            id, request.Name, request.Code, request.Address,
            request.Phone, request.Email, request.IsActive
        );
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a branch
    /// </summary>
    /// <remarks>Requires Admin role</remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBranch(
        Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBranchCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
